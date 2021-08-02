using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace PSA.Saver
{
    public class PsaConfigSection : ConfigurationSection
    {
        const string ElementName = "PsaConfig";

        [ConfigurationProperty(ElementName)]
        public PsaConfigElement Config {
            get { return (PsaConfigElement)this[ElementName]; }
            set { this[ElementName] = value; }
        }
    };

}
