using System.Configuration;

namespace EntrustHipChat
{
    public class EntrustInstanceConfigurationElement : ConfigurationElement
    {
		/// <summary>
		/// Gets or sets a name for the instance
		/// </summary>
		[ConfigurationProperty("name", IsRequired = true)]
		public string Name
		{
			get { return (string)this["name"]; }
			set { this["name"] = value; }
		}

        /// <summary>
        /// Gets or sets the configuration information about Entrust
        /// </summary>
        [ConfigurationProperty("entrust", IsRequired = true)]
        public EntrustConfigurationElement Entrust
        {
			get { return (EntrustConfigurationElement)this["entrust"]; }
			set { this["entrust"] = value; }
        }

        /// <summary>
        /// Gets or sets the configuration information for HipChat
        /// </summary>
        [ConfigurationProperty("hipChat", IsRequired = false)]
        public HipChatConfigurationElement HipChat
        {
			get { return (HipChatConfigurationElement)this["hipChat"]; }
			set { this["hipChat"] = value; }
        }

		/// <summary>
		/// Gets or sets the configuration information for HipChat
		/// </summary>
		[ConfigurationProperty("pagerDuty", IsRequired = false)]
		public PagerDutyConfigurationElement PagerDuty
		{
			get { return (PagerDutyConfigurationElement)this["pagerDuty"]; }
			set { this["pagerDuty"] = value; }
		}
    }
}
