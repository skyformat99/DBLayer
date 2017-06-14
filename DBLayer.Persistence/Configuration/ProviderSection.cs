using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using DBLayer.Persistence.Configuration.Provider;

namespace DBLayer.Persistence.Configuration
{
    public class ProviderSection : ConfigurationSection
    {
        public const string SectionName = "atom/provider";


        private const string xmlnsXmlKey = "xmlns";

        [ConfigurationProperty(xmlnsXmlKey, IsRequired = false, DefaultValue = "urn:provider")]
        public string Xmlns
        {
            get { return (string)this[xmlnsXmlKey]; }
            set { this[xmlnsXmlKey] = value; }
        }

        [ConfigurationProperty(ProviderElementCollection.CollectionXmlName, IsRequired = true)]
        public ProviderElementCollection AtomProviders
        {
            get { return this[ProviderElementCollection.CollectionXmlName] as ProviderElementCollection; }
            set
            {
                this[ProviderElementCollection.CollectionXmlName] = value;
            }
        }
    }
}
