using System.Configuration;

namespace EntrustHipChat
{
	public class EntrustConfigurationElement : ConfigurationElement
	{
		[ConfigurationProperty("licenseWarn", IsRequired = true)]
		public int LicenseWarn
		{
			get { return (int)this["licenseWarn"]; }
			set { this["licenseWarn"] = value; }
		}

		[ConfigurationProperty("licenseMax", IsRequired = true)]
		public int LicenseMax
		{
			get { return (int)this["licenseMax"]; }
			set { this["licenseMax"] = value; }
		}

		[ConfigurationProperty("connectionString", IsRequired = true)]
		public string ConnectionString
		{
			get { return (string)this["connectionString"]; }
			set { this["connectionString"] = value; }
		}
	}
}
