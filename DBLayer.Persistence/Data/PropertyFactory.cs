

using System.Collections.Generic;
using DBLayer.Persistence.Configuration;
using DBLayer.Persistence.Configuration.Provider;
using DBLayer.Persistence.Configuration.Property;
using System;

namespace DBLayer.Persistence.Data
{
    /// <summary>
    /// Create DbProviders based on configuration information from resource
    /// providers.config
    /// </summary>
    public class PropertyFactory
    {
        private static PropertyFactory _settings;


        private readonly IDictionary<string, Property> repository = new Dictionary<string, Property>();
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DbProviderFactory"/> class.
        /// </summary>
        /// <param name="providers">The providers.</param>
        private PropertyFactory()
        {
            foreach (PropertyElement config in PropertySettings.Instance.ConfigSection.AtomProperties)
            {
                Property provider = PropertyDeSerializer.Deserialize(config);
                repository.Add(config.Name, provider);
            }
        }

        /// <summary>
        /// 单例模式
        /// </summary>
        public static PropertyFactory Instance
        {
            get
            {
                if (_settings == null)
                {
                    //double check
                    if (_settings == null)
                    {
                        _settings = new PropertyFactory();
                    }
                }
                return _settings;
            }
        }

        /// <summary>
        /// Gets the IDbProvider given an identifying name.
        /// </summary>
        /// <remarks>
        /// Familiar names for the .NET 2.0 property model are supported, i.e.
        /// System.Data.SqlClient.  Refer to the documentation for a complete
        /// listing of supported DbProviders and their names.  
        /// </remarks>
        /// <param name="providerInvariantName">Name of the property invariant.</param>
        /// <returns>An IDbProvider</returns>
        public Property GetProperty(string propertyInvariantName)
        {
            if (repository.ContainsKey(propertyInvariantName))
            {
                return repository[propertyInvariantName];
            }
            throw new Exception("There's no property with the name '" + propertyInvariantName + "'. Cause you give a wrong name or the property is disabled in the providers.config file.");
        }
    }
}
