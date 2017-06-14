
#region Using
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Xml;
using DBLayer.Persistence.Configuration;
using DBLayer.Persistence.Configuration.AtomSource;
using DBLayer.Core.Interface;
using DBLayer.Persistence.Utilities;
using DBLayer.Core;
#endregion 

namespace DBLayer.Persistence.Data
{
	/// <summary>
	/// Summary description for DataSourceDeSerializer.
	/// </summary>
	public sealed class DataSourceDeSerializer
	{
        /// <summary>
        /// Deserialize a DataSource object
        /// </summary>
        /// <param name="dbProvider">The db provider.</param>
        /// <param name="commandTimeOut">The command time out.</param>
        /// <param name="config">The config.</param>
        /// <returns></returns>
        public static DataSource Deserialize(AtomSourceElement config)
		{
            
            var name = config.Name;
            var providerName=config.Provider;
            var propertyName = config.Property;
            var dbProvider=DbProviderFactory.Instance.GetDbProvider(providerName);
            var property=PropertyFactory.Instance.GetProperty(propertyName);

            if (property.AllKeys.Contains(PropertyConstants.PASSWORDKEY)) 
            {
                var passwordKey = property[PropertyConstants.PASSWORDKEY];
                if (!string.IsNullOrEmpty(passwordKey)) 
                {
                    var password = property[PropertyConstants.PASSWORD];
                    //Ω‚√‹
                    property[PropertyConstants.PASSWORD] = AES.Decode(password, passwordKey);
                }
            }

            //config.Property;
            var connectionString = new ConnectionString() 
            {
                ConnectionToken = config.ConnectionString,
                Properties = property
            };

            return new DataSource() 
            {
                ConnectionString = connectionString,
                DbProvider = dbProvider
            };
		}


	}
}
