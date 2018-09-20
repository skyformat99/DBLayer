using System.Configuration;

namespace DBLayer.Persistence.Configuration.Property
{

    public class PropertyElement : ConfigurationElement
    {
        #region 基本配置
        public const string ElementXmlName = "property";

        private const string nameXmlKey = "name";


        [ConfigurationProperty(nameXmlKey, IsKey = true, IsRequired = true)]
        public string Name { get { return (string)this[nameXmlKey]; } set { this[nameXmlKey] = value; } }


        [ConfigurationProperty(AddElementCollection.CollectionXmlName, IsRequired = true)]
        public AddElementCollection Settings
        {
            get { return this[AddElementCollection.CollectionXmlName] as AddElementCollection; }
            set
            {
                this[AddElementCollection.CollectionXmlName] = value;
            }
        }

        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}
