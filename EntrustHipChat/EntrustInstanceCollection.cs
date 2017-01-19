using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace EntrustHipChat
{
	public class EntrustInstanceCollection : ConfigurationElementCollection, IEnumerable<EntrustInstanceConfigurationElement>
    {
        protected override ConfigurationElement CreateNewElement()
        {
			return new EntrustInstanceConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
			return ((EntrustInstanceConfigurationElement)element).Name;
        }

		public EntrustInstanceConfigurationElement this[int index]
        {
			get { return (EntrustInstanceConfigurationElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

		public void Add(EntrustInstanceConfigurationElement serviceConfig)
        {
            BaseAdd(serviceConfig);
        }

        public void Clear()
        {
            BaseClear();
        }

		public void Remove(EntrustInstanceConfigurationElement serviceConfig)
        {
            BaseRemove(serviceConfig.Name);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }

		public new IEnumerator<EntrustInstanceConfigurationElement> GetEnumerator()
        {
			return BaseGetAllKeys().Select(key => (EntrustInstanceConfigurationElement)BaseGet(key)).GetEnumerator();
        }
    }
}
