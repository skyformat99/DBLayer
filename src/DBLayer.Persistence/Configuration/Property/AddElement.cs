using System.Configuration;

namespace DBLayer.Persistence.Configuration.Property
{

    public class AddElement : ConfigurationElement
    {
        #region 基本配置
        public const string ElementXmlName = "add";

        private const string keyXmlKey = "key";
        private const string valueXmlKey = "value";


        [ConfigurationProperty(keyXmlKey, IsKey = true, IsRequired = true)]
        public string Key { get { return (string)this[keyXmlKey]; } set { this[keyXmlKey] = value; } }

        [ConfigurationProperty(valueXmlKey, IsKey = true, IsRequired = true)]
        public string Value { get { return (string)this[valueXmlKey]; } set { this[valueXmlKey] = value; } }

        public override string ToString()
        {
            return Key;
        }

        #endregion
    }
}
