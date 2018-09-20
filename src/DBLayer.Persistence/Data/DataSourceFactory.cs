
using DBLayer.Core.Interface;
using DBLayer.Persistence.Configuration;
using DBLayer.Persistence.Configuration.AtomSource;
using System;
using System.Collections.Generic;

namespace DBLayer.Persistence.Data
{
    /// <summary>
    /// Create DataSources based on configuration information from resource
    /// providers.config
    /// </summary>
    public class DataSourceFactory
    {
        private static DataSourceFactory _settings;

        private readonly IDictionary<string, IDataSource> repository = new Dictionary<string, IDataSource>();
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DbProviderFactory"/> class.
        /// </summary>
        /// <param name="providers">The providers.</param>
        private DataSourceFactory()
        {
            foreach (AtomSourceElement config in AtomSourceSettings.Instance.ConfigSection.AtomDataSources)
            {
                var source = DataSourceDeSerializer.Deserialize(config);
                repository.Add(config.Name, source);
            }
        }
        /// <summary>
        /// 单例模式
        /// </summary>
        public static DataSourceFactory Instance
        {
            get
            {
                if (_settings == null)
                {
                    //double check
                    if (_settings == null)
                    {
                        _settings = new DataSourceFactory();
                    }
                }
                return _settings;
            }
        }
        /// <summary>
        /// Gets the IDataSource given an identifying name.
        /// </summary>
        /// <remarks>
        /// Familiar names for the .NET 2.0 datasource model are supported, i.e.
        /// System.Data.SqlClient.  Refer to the documentation for a complete
        /// listing of supported DataSources and their names.  
        /// </remarks>
        /// <param name="datsourceInvariantName">Name of the datasource invariant.</param>
        /// <returns>An IDbProvider</returns>
        public IDataSource GetDataSource(string datasourceInvariantName)
        {
            if (repository.ContainsKey(datasourceInvariantName))
            {
                return repository[datasourceInvariantName];
            }
            throw new Exception("There's no datasource with the name '" + datasourceInvariantName + "'. Cause you give a wrong name or the datasource is disabled in the providers.config file.");
        }
    }
}
