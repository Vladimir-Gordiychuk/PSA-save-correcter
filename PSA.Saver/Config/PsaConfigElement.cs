using System.Configuration;
using System.IO;

namespace PSA.Saver
{
    public class PsaConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("AttrConvPath")]
        public string AttrConvPath
        {
            get
            {
                if (this["AttrConvPath"] != null)
                {
                    return (string)this["AttrConvPath"];
                }
                return null;
            }
        }

        [ConfigurationProperty("SavePath")]
        public string SavePath
        {
            get
            {
                if (this["SavePath"] != null)
                {
                    return (string)this["SavePath"];
                }
                return null;
            }
        }

        public FileInfo AttrConvExecutable
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(AttrConvPath))
                {
                    try
                    {
                        return new FileInfo(AttrConvPath);
                    }
                    catch
                    {
                        return null;
                    }
                }
                return null;
            }
        }

        public DirectoryInfo SaveDirectory
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(SavePath))
                {
                    try
                    {
                        return new DirectoryInfo(SavePath);
                    }
                    catch
                    {
                        return null;
                    }
                }
                return null;
            }
        }

    };

}
