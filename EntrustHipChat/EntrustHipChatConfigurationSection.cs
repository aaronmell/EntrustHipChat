using System.Configuration;

namespace EntrustHipChat
{
    public class EntrustHipChatConfigurationSection : ConfigurationSection
    {
        /// <summary>
        /// Configuration information for alerting criteria
        /// </summary>
        [ConfigurationProperty("instances", IsDefaultCollection = false)]
		[ConfigurationCollection(typeof(EntrustInstanceCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
		public EntrustInstanceCollection Instances
        {
            get
            {
				return (EntrustInstanceCollection)base["instances"];
            }
        }
    }
}
