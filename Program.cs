// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Copyright Sean McElroy">
//   Copyright Sean McElroy
// </copyright>
// <summary>
//   Defines the primary class and entry point for running the console application
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;

namespace EntrustHipChat
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Data;
	using System.Data.SqlClient;
	using System.IO;
	using System.Net;
	using System.Text;
	using System.Threading;
	using HipChat;
	using Newtonsoft.Json;

	class Program
	{
		internal static void Main(string[] args)
		{
			var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			var entrustHipChatConfigurationSection = (EntrustHipChatConfigurationSection)config.GetSection("entrustHipChat");
			var pagerDutyAlerts = new Dictionary<string, string>();

			var allOkay = true;

			foreach (var hc in entrustHipChatConfigurationSection.Instances.Select(p => new { p.HipChat.HipChatApiKey, p.HipChat.HipChatRoomName }).Distinct())
				HipChatClient.SendMessage(hc.HipChatApiKey, hc.HipChatRoomName, "EntrustBot", "EntrustBot has started", true, HipChatClient.BackgroundColor.purple);

			while (true)
			{
				try
				{
					foreach (var instance in entrustHipChatConfigurationSection.Instances)
					{
						int? userCount = null;

						try
						{
							using (var conn = new SqlConnection(instance.Entrust.ConnectionString))
							{
								try
								{
									conn.Open();

									using (var comm = new SqlCommand("select COUNT(u.[user_id]) from users u WITH (NOLOCK)", conn))
									{
										using (var reader = comm.ExecuteReader(CommandBehavior.SingleResult))
										{
											if (reader.Read())
											{
												var columns = new object[reader.FieldCount];
												reader.GetValues(columns);
												userCount = (int)columns[0];
											}

											reader.Close();
										}
									}
								}
								catch (Exception)
								{
									Console.WriteLine("PROBLEM CONNECTING TO: {0}", instance.Entrust.ConnectionString);
									throw;
								}
								finally
								{
									conn.Close();
								}
							}

							if (!userCount.HasValue)
							{
								var message = string.Format("Unable to retrieve the license count for {0}.", instance.Name);
								Console.WriteLine(message);
								if (instance.HipChat != null)
									HipChatClient.SendMessage(instance.HipChat.HipChatApiKey, instance.HipChat.HipChatRoomName, "EntrustBot", message, true, HipChatClient.BackgroundColor.yellow);
								allOkay = false;
							}
							else if (userCount >= instance.Entrust.LicenseMax)
							{
								var message = string.Format("CAL count for {0} is {1:N0}!  INSTANCE MAY BE LOCKED FOR NEW REGISTRATIONS!  Limit is {2:N0}.", instance.Name, userCount.Value, instance.Entrust.LicenseMax);

								if (instance.HipChat != null) 
									HipChatClient.SendMessage(instance.HipChat.HipChatApiKey, instance.HipChat.HipChatRoomName, "EntrustBot", message, true, HipChatClient.BackgroundColor.red);

								if (instance.PagerDuty != null && (!pagerDutyAlerts.ContainsKey(instance.Name) || pagerDutyAlerts[instance.Name] == "WARN"))
								{
									PostPagerDutyAlert(instance.PagerDuty.GenericServiceApiKey, message, userCount.Value);
									pagerDutyAlerts[instance.Name] = "FAIL";
								}

								allOkay = false;
							}
							else if (userCount >= instance.Entrust.LicenseWarn)
							{
								var message = string.Format("CAL count for {0} is {1:N0}.  Above warning threshold of {2:N0}.  Limit is {3:N0}.", instance.Name, userCount.Value, instance.Entrust.LicenseWarn, instance.Entrust.LicenseMax);
								if (instance.HipChat != null) 
									HipChatClient.SendMessage(instance.HipChat.HipChatApiKey, instance.HipChat.HipChatRoomName, "EntrustBot", message, true, HipChatClient.BackgroundColor.yellow);

								//if (instance.PagerDuty != null && !pagerDutyAlerts.ContainsKey(instance.Name))
								//{
								//    PostPagerDutyAlert(instance.PagerDuty.GenericServiceApiKey, message, userCount.Value);
								//    pagerDutyAlerts[instance.Name] = "WARN";
								//}
								
								allOkay = false;
							}
							else
								Console.WriteLine("CAL count for {0} is {1:N0}.  Limit is {2:N0}.", instance.Name, userCount.Value, instance.Entrust.LicenseMax);
						}
						catch (Exception e)
						{
							Console.WriteLine(e.ToString());
							allOkay = false;
						}
					}

					if (allOkay)
					{
						Console.WriteLine("All {0} Entrust licensing checks passed successfully.", entrustHipChatConfigurationSection.Instances.Count);
						pagerDutyAlerts.Clear();
					}
				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
				}

				Thread.Sleep(new TimeSpan(1, 0, 0));
			}
		}

		private static void PostPagerDutyAlert(string pagerDutyServiceKey, string description, float cur)
		{
			Console.WriteLine("[{0} {1}] Posting alert to PagerDuty for {2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), description);

			var json = new
			{
				service_key = pagerDutyServiceKey,
				event_type = "trigger",
				description,
				details = new
				{
					current_average = cur
				}
			};

			var http = (HttpWebRequest)WebRequest.Create(new Uri("https://events.pagerduty.com/generic/2010-04-15/create_event.json"));
			http.Accept = "application/json";
			http.ContentType = "application/json";
			http.Method = "POST";

			var parsedContent = JsonConvert.SerializeObject(json);
			var encoding = new ASCIIEncoding();
			var bytes = encoding.GetBytes(parsedContent);

			var newStream = http.GetRequestStream();
			newStream.Write(bytes, 0, bytes.Length);
			newStream.Close();

			var response = http.GetResponse();
			using (var stream = response.GetResponseStream())
			{
				if (stream != null)
				{
					using (var sr = new StreamReader(stream))
					{
						var content = sr.ReadToEnd();
						Console.WriteLine("Response from PagerDuty: {0}", content);
					}
				}
			}
		}
	}
}
