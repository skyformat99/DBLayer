using System.Configuration;

namespace DBLayer.Persistence.Configuration.AtomSource
{

    public class AtomSourceElement : ConfigurationElement
    {
        #region 基本配置
        public const string ElementXmlName = "atom-datasource";

        private const string nameXmlKey = "name";
        private const string connectionStringXmlKey = "connectionString";
        private const string propertyXmlKey = "property";
        private const string providerXmlKey = "provider";


        [ConfigurationProperty(nameXmlKey, IsKey = true, IsRequired = true)]
        public string Name { get { return (string)this[nameXmlKey]; } set { this[nameXmlKey] = value; } }

        [ConfigurationProperty(connectionStringXmlKey, IsKey = true, IsRequired = true)]
        public string ConnectionString { get { return (string)this[connectionStringXmlKey]; } set { this[connectionStringXmlKey] = value; } }

        [ConfigurationProperty(propertyXmlKey, IsKey = true, IsRequired = true)]
        public string Property { get { return (string)this[propertyXmlKey]; } set { this[propertyXmlKey] = value; } }

        [ConfigurationProperty(providerXmlKey, IsKey = true, IsRequired = true)]
        public string Provider { get { return (string)this[providerXmlKey]; } set { this[providerXmlKey] = value; } }

        
        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}
