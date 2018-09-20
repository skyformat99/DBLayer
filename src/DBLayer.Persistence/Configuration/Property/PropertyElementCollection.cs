using DBLayer.Persistence.Configuration.Base;
using System.Configuration;

namespace DBLayer.Persistence.Configuration.Property
{
    [ConfigurationCollection(typeof(PropertyElement), AddItemName = PropertyElement.ElementXmlName)]
    public class PropertyElementCollection : ConfigurationElementCollection<string, PropertyElement>
    {
        #region 基本配置
        public const string CollectionXmlName = "properties";
        #endregion

        /// <summary>
        /// 获得 <see cref="T:System.Configuration.ConfigurationElementCollection"/>类型.
        /// </summary>
        /// <returns>
        ///  <see cref="T:System.Configuration.ConfigurationElementCollectionType"/> 类型集合
        /// </returns>
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }

        /// <summary>
        /// 获取用于识别这个元素在配置文件中的集合在派生类中重写时名称。
        /// </summary>
        /// <returns>
        /// 集合的名称，否则，一个空字符串。默认是一个空字符串。
        /// </returns>
        protected override string ElementName
        {
            get { return CollectionXmlName; }
        }


        protected override string GetElementKey(PropertyElement element)
        {
            return element.ToString();
        }

        
    }
}
