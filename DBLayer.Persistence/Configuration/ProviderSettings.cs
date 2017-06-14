using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;

namespace DBLayer.Persistence.Configuration
{
    public class ProviderSettings
    {
        #region 单例模式
        private static ProviderSettings _settings;
        private static readonly object Lock = new object();

        internal ProviderSettings()
        {
            if (ConfigSection == null)
            {
                ConfigSection = (ProviderSection)ConfigurationManager
                    .GetSection(ProviderSection.SectionName);
            }
        }

        internal ProviderSettings(FileInfo configFile)
        {
            if (!configFile.Exists)
            {
                throw new ArgumentException("The configuration file specified doesn't exist", "configFile");
            }
            
            var fileMap = new ExeConfigurationFileMap { ExeConfigFilename = configFile.FullName };
            var configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            ConfigSection = (ProviderSection)configuration
                .GetSection(ProviderSection.SectionName);
        }

        /// <summary>
        /// 单例模式
        /// </summary>
        public static ProviderSettings Instance
        {
            get
            {
                if (_settings == null)
                {
                    lock (Lock)
                    {
                        //double check
                        if (_settings == null)
                        {
                            _settings = new ProviderSettings();
                        }
                    }
                }
                return _settings;
            }
        }
        #endregion
        
        #region 属性
        public ProviderSection ConfigSection { get; private set; }

	    #endregion
    }
}
