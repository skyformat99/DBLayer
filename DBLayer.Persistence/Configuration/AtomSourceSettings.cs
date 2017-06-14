using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;

namespace DBLayer.Persistence.Configuration
{
    public class AtomSourceSettings
    {
        #region 单例模式
        private static AtomSourceSettings _settings;
        private static readonly object Lock = new object();

        internal AtomSourceSettings()
        {
            if (ConfigSection == null)
            {
                ConfigSection = (AtomSourceSection)ConfigurationManager
                    .GetSection(AtomSourceSection.SectionName);
            }
        }

        internal AtomSourceSettings(FileInfo configFile)
        {
            if (!configFile.Exists)
            {
                throw new ArgumentException("The configuration file specified doesn't exist", "configFile");
            }
            
            var fileMap = new ExeConfigurationFileMap { ExeConfigFilename = configFile.FullName };
            var configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            
            ConfigSection = (AtomSourceSection)configuration
                .GetSection(AtomSourceSection.SectionName);
        }

        /// <summary>
        /// 单例模式
        /// </summary>
        public static AtomSourceSettings Instance
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
                            _settings = new AtomSourceSettings();
                        }
                    }
                }
                return _settings;
            }
        }
        #endregion
        
        #region 属性
        public AtomSourceSection ConfigSection { get; private set; }

	    #endregion
    }
}
