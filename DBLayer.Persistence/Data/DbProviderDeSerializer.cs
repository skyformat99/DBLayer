
#region Using

using System.Collections.Specialized;
using System.Xml;
using DBLayer.Persistence.Configuration.Provider;
using DBLayer.Core.Interface;
#endregion 

namespace DBLayer.Persistence.Data
{

    public sealed class DbProviderDeSerializer
    {

        /// <summary>
        /// Deserializes the specified node in a <see cref="IDbProvider"/>.
        /// </summary>
        /// <param name="config">The IConfiguration node.</param>
        /// <returns>The <see cref="IDbProvider"/></returns>
        public static IDbProvider Deserialize(ProviderElement config)
        {
            var provider = new DbProvider() {
                ParameterPrefix = config.ParameterPrefix,
                ProviderName = config.ProviderName,
                SelectKey = config.SelectKey
            };
            return provider;
        }
    }
}
