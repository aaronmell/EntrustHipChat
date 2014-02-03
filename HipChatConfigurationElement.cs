﻿using System.Configuration;

namespace EntrustHipChat
{
    /// <summary>
    /// Configuration information for PagerDuty
    /// </summary>
    public class HipChatConfigurationElement : ConfigurationElement
    {
        /// <summary>
        /// The API key of the PagerDuty 'generic service' to dispatch alerts to
        /// </summary>
		[ConfigurationProperty("hipChatAPIKey", IsRequired = true)]
        public string HipChatApiKey
        {
			get { return (string)this["hipChatAPIKey"]; }
			set { this["hipChatAPIKey"] = value; }
        }

        /// <summary>
        /// The message to send to PagerDuty on alert
        /// </summary>
        [ConfigurationProperty("hipChatRoomName", IsRequired = true)]
		public string HipChatRoomName
        {
			get { return (string)this["hipChatRoomName"]; }
			set { this["hipChatRoomName"] = value; }
        }
    }
}
