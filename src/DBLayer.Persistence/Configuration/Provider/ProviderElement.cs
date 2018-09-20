using System.Configuration;

namespace DBLayer.Persistence.Configuration.Provider
{

    public class ProviderElement : ConfigurationElement
    {
        #region 基本配置
        public const string ElementXmlName = "provider";

        private const string nameXmlKey = "name";
        private const string providerNameXmlKey = "providerName";
        private const string parameterPrefixXmlKey = "parameterPrefix";
        private const string selectKeyXmlKey = "selectKey";

        [ConfigurationProperty(nameXmlKey, IsKey = true, IsRequired = true)]
        public string Name { get { return (string)this[nameXmlKey]; } set { this[nameXmlKey] = value; } }

        [ConfigurationProperty(providerNameXmlKey, IsKey = true, IsRequired = true)]
        public string ProviderName { get { return (string)this[providerNameXmlKey]; } set { this[providerNameXmlKey] = value; } }

        [ConfigurationProperty(parameterPrefixXmlKey, IsKey = false)]
        public string ParameterPrefix { get { return (string)this[parameterPrefixXmlKey]; } set { this[parameterPrefixXmlKey] = value; } }

        [ConfigurationProperty(selectKeyXmlKey, IsKey = false)]
        public string SelectKey { get { return (string)this[selectKeyXmlKey]; } set { this[selectKeyXmlKey] = value; } }
        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}
