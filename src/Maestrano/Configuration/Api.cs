using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maestrano.Configuration
{
    public class Api : ConfigurationSection
    {

        /// <summary>
        /// Load Api configuration into a Api configuration object
        /// </summary>
        /// <returns>A Api configuration object</returns>
        public static Api Load()
        {
            var config = ConfigurationManager.GetSection("maestrano/api") as Api;
            if (config == null) config = new Api();

            return config;
        }

        /// <summary>
        /// Return False (object not read only)
        /// </summary>
        /// <returns></returns>
        public override bool IsReadOnly()
        {
            return false;
        }

        /// <summary>
        /// API Id (from sandbox or maestrano.com)
        /// </summary>
        [ConfigurationProperty("id", DefaultValue = null, IsRequired = false)]
        public String Id
        {
            get { return (String)this["id"]; }
            set { this["id"] = value; }
        }

        /// <summary>
        /// API Key (from sandbox or maestrano.com)
        /// </summary>
        [ConfigurationProperty("key", DefaultValue = null, IsRequired = false)]
        public String Key
        {
            get { return (String)this["key"]; }
            set { this["key"] = value; }
        }

        /// <summary>
        /// API Key (from sandbox or maestrano.com)
        /// </summary>
        [ConfigurationProperty("base", DefaultValue = "/api/v1/", IsRequired = false)]
        public String Base
        {
            get { return (String)this["base"]; }
            set { this["base"] = value; }
        }

        /// <summary>
        /// Verify SSL certs (disabled for now)
        /// </summary>
        [ConfigurationProperty("verifySslCerts", DefaultValue = false, IsRequired = false)]
        public bool VerifySslCerts
        {
            get { return (Boolean)this["verifySslCerts"]; }
            set { this["verifySslCerts"] = value; }
        }

        // Return an identification token ready to
        // be used for HTTP auth
        public string Token { get { return Id + ':' + Key; } }

        // Language used by the API
        public string Lang { get { return "C#"; } }

        // Return the API version
        public string Version { get { return MnoHelper.Version; } }

        // Return the language version
        public string LangVersion { get { return Environment.OSVersion.ToString() + " - " + Environment.Version.ToString(); } }

        /// <summary>
        /// Address of the API Host
        /// </summary>
        [ConfigurationProperty("host", DefaultValue = null, IsRequired = false)]
        public string Host
        {
            get
            {
                var _idp = (String)this["host"];
                if (string.IsNullOrEmpty(_idp))
                {
                    if (MnoHelper.Environment.Equals("production"))
                    {
                        return "https://maestrano.com";
                    }
                    else
                    {
                        return "https://mno-api-sandbox.herokuapp.com"; // "http://api-sandbox.maestrano.io";
                    }
                }
                return _idp;
            }

            set { this["host"] = value; }
        }

    }
}
