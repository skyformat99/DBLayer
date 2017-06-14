
using System.Collections.Generic;
using DBLayer.Persistence.Configuration;
using DBLayer.Persistence.Configuration.Provider;
using DBLayer.Core.Interface;
using System;

namespace DBLayer.Persistence.Data
{
    /// <summary>
    /// Create DbProviders based on configuration information from resource
    /// providers.config
    /// </summary>
    public class DbProviderFactory
    {
        private static DbProviderFactory _settings;

        private readonly IDictionary<string, IDbProvider> repository = new Dictionary<string, IDbProvider>();
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DbProviderFactory"/> class.
        /// </summary>
        /// <param name="providers">The providers.</param>
        private DbProviderFactory()
        {
            foreach (ProviderElement config in ProviderSettings.Instance.ConfigSection.AtomProviders)
            {
                var provider = DbProviderDeSerializer.Deserialize(config);
                repository.Add(config.Name, provider);
            }
        }

        /// <summary>
        /// 单例模式
        /// </summary>
        public static DbProviderFactory Instance
        {
            get
            {
                if (_settings == null)
                {
                    //double check
                    if (_settings == null)
                    {
                        _settings = new DbProviderFactory();
                    }
                }
                return _settings;
            }
        }

        /// <summary>
        /// Gets the IDbProvider given an identifying name.
        /// </summary>
        /// <remarks>
        /// Familiar names for the .NET 2.0 provider model are supported, i.e.
        /// System.Data.SqlClient.  Refer to the documentation for a complete
        /// listing of supported DbProviders and their names.  
        /// </remarks>
        /// <param name="providerInvariantName">Name of the provider invariant.</param>
        /// <returns>An IDbProvider</returns>
        public IDbProvider GetDbProvider(string providerInvariantName)
        {
            if (repository.ContainsKey(providerInvariantName))
            {
                return repository[providerInvariantName];
            }
            throw new Exception("There's no provider with the name '" + providerInvariantName + "'. Cause you give a wrong name or the provider is disabled in the providers.config file.");
        }
    }
}
