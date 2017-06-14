
#region Using

using System.Collections.Specialized;
using System.Xml;
using DBLayer.Persistence.Configuration.Provider;
using DBLayer.Persistence.Configuration.Property;
#endregion 

namespace DBLayer.Persistence.Data
{

    public sealed class PropertyDeSerializer
    {

        /// <summary>
        /// Deserializes the specified node in a <see cref="IDbProvider"/>.
        /// </summary>
        /// <param name="config">The IConfiguration node.</param>
        /// <returns>The <see cref="IDbProvider"/></returns>
        public static Property Deserialize(PropertyElement config)
        {
            var p = new Property();
            foreach (AddElement item in config.Settings)
            {
                p.Add(item.Key,item.Value);
            }
            return p;
        }
    }
}
