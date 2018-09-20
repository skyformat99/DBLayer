using System;
using System.Configuration;
using System.IO;

namespace DBLayer.Persistence.Configuration
{
    public class PropertySettings
    {
        #region 单例模式
        private static PropertySettings _settings;
        private static readonly object Lock = new object();

        internal PropertySettings()
        {
            if (ConfigSection == null)
            {
                ConfigSection = (PropertySection)ConfigurationManager
                    .GetSection(PropertySection.SectionName);
            }
        }

        internal PropertySettings(FileInfo configFile)
        {
            if (!configFile.Exists)
            {
                throw new ArgumentException("The configuration file specified doesn't exist", "configFile");
            }
            
            var fileMap = new ExeConfigurationFileMap { ExeConfigFilename = configFile.FullName };
            var configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            ConfigSection = (PropertySection)configuration
                .GetSection(PropertySection.SectionName);
        }

        /// <summary>
        /// 单例模式
        /// </summary>
        public static PropertySettings Instance
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
                            _settings = new PropertySettings();
                        }
                    }
                }
                return _settings;
            }
        }
        #endregion
        
        #region 属性
        public PropertySection ConfigSection { get; private set; }

	    #endregion
    }
}
