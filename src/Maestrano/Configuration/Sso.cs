using Maestrano.Sso;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.SessionState;

namespace Maestrano.Configuration
{
    public class Sso : ConfigurationSection
    {
        /// <summary>
        /// Load Sso configuration into a Sso configuration object
        /// </summary>
        /// <returns>A Sso configuration object</returns>
        public static Sso Load()
        {
            var config = ConfigurationManager.GetSection("maestrano/sso") as Sso;
            if (config == null) config = new Sso();

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
        /// Is Single Sign-On enabled - useful for debugging
        /// </summary>
        [ConfigurationProperty("enabled", DefaultValue = true, IsRequired = false)]
        public bool Enabled
        { 
            get { return (Boolean)this["enabled"]; }
            set { this["enabled"] = value; }
        }

        /// <summary>
        /// Is Single Logout enabled - useful for debugging
        /// </summary>
        [ConfigurationProperty("sloEnabled", DefaultValue = true, IsRequired = false)]
        public bool SloEnabled
        {
            get { return (Boolean)this["sloEnabled"]; }
            set { this["sloEnabled"] = value; }
        }

        /// <summary>
        /// SSO user creation mode: 'real' or 'virtual'
        /// </summary>
        [ConfigurationProperty("creationMode", DefaultValue = "virtual", IsRequired = false)]
        public string CreationMode
        {
            get { return (String)this["creationMode"]; }
            set { this["creationMode"] = value; }
        }

        /// <summary>
        /// Path to init action
        /// </summary>
        [ConfigurationProperty("initPath", DefaultValue = "/maestrano/auth/saml/init.aspx", IsRequired = false)]
        public string InitPath
        {
            get { return (String)this["initPath"]; }
            set { this["initPath"] = value; }
        }

        /// <summary>
        /// Path to consume action
        /// </summary>
        [ConfigurationProperty("consumePath", DefaultValue = "/maestrano/auth/saml/consume.aspx", IsRequired = false)]
        public string ConsumePath
        {
            get { return (String)this["consumePath"]; }
            set { this["consumePath"] = value; }
        }

        /// <summary>
        /// Address of the identity manager (for your application)
        /// </summary>
        [ConfigurationProperty("idm", DefaultValue = null, IsRequired = false)]
        public string Idm
        {
            get
            {
                var _idm = (String)this["idm"];
                if (string.IsNullOrEmpty(_idm))
                {
                    if (!string.IsNullOrEmpty(MnoHelper.App.Host))
                    {
                        return MnoHelper.App.Host;
                    }
                    return "localhost";
                }
                return _idm;
            }

            set { this["idm"] = value; }
        }

        /// <summary>
        /// Address of the identity provider
        /// </summary>
        [ConfigurationProperty("idp", DefaultValue = null, IsRequired = false)]
        public string Idp
        { 
            get
            {
                var _idp = (String)this["idp"];
                if(string.IsNullOrEmpty(_idp)) {
                    if (MnoHelper.Environment.Equals("production")) {
                        return "https://maestrano.com";
                    } else {
                        return "https://mno-api-sandbox.herokuapp.com"; // "http://api-sandbox.maestrano.io";
                    }
                }
                return _idp;
            }

            set { this["idp"] = value; }
        }

        
        /// <summary>
        /// The nameid format for SAML handshake
        /// </summary>
        [ConfigurationProperty("nameIdFormat", DefaultValue = "urn:oasis:names:tc:SAML:2.0:nameid-format:persistent", IsRequired = false)]
        public string NameIdFormat 
        {
            get { return (String)this["nameIdFormat"]; }
            set { this["nameIdFormat"] = value; }
        }

        
        /// <summary>
        /// Fingerprint of x509 certificate used for SAML
        /// </summary>
        [ConfigurationProperty("x509Fingerprint", DefaultValue = null, IsRequired = false)]
        public string X509Fingerprint
        { 
            get
            {
                var _x509fingerprint = (String)this["x509Fingerprint"];
                if(string.IsNullOrEmpty(_x509fingerprint)) {
                    if (MnoHelper.Environment.Equals("production")) {
                        return "2f:57:71:e4:40:19:57:37:a6:2c:f0:c5:82:52:2f:2e:41:b7:9d:7e";
                    } else {
                        return "01:06:15:89:25:7d:78:12:28:a6:69:c7:de:63:ed:74:21:f9:f5:36";
                    }
                }
                return _x509fingerprint;
            }

            set { this["x509Fingerprint"] = value; }
        }

        // Actual x509 certificate
        [ConfigurationProperty("x509Certificate", DefaultValue = null, IsRequired = false)]
        public string X509Certificate
        { 
            get
            {
                var _509certificate = (String)this["x509Certificate"];
                if (string.IsNullOrEmpty(_509certificate))
                {
                    if (MnoHelper.Environment.Equals("production")) {
                        return "-----BEGIN CERTIFICATE-----\nMIIDezCCAuSgAwIBAgIJAPFpcH2rW0pyMA0GCSqGSIb3DQEBBQUAMIGGMQswCQYD\nVQQGEwJBVTEMMAoGA1UECBMDTlNXMQ8wDQYDVQQHEwZTeWRuZXkxGjAYBgNVBAoT\nEU1hZXN0cmFubyBQdHkgTHRkMRYwFAYDVQQDEw1tYWVzdHJhbm8uY29tMSQwIgYJ\nKoZIhvcNAQkBFhVzdXBwb3J0QG1hZXN0cmFuby5jb20wHhcNMTQwMTA0MDUyNDEw\nWhcNMzMxMjMwMDUyNDEwWjCBhjELMAkGA1UEBhMCQVUxDDAKBgNVBAgTA05TVzEP\nMA0GA1UEBxMGU3lkbmV5MRowGAYDVQQKExFNYWVzdHJhbm8gUHR5IEx0ZDEWMBQG\nA1UEAxMNbWFlc3RyYW5vLmNvbTEkMCIGCSqGSIb3DQEJARYVc3VwcG9ydEBtYWVz\ndHJhbm8uY29tMIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQD3feNNn2xfEz5/\nQvkBIu2keh9NNhobpre8U4r1qC7h7OeInTldmxGL4cLHw4ZAqKbJVrlFWqNevM5V\nZBkDe4mjuVkK6rYK1ZK7eVk59BicRksVKRmdhXbANk/C5sESUsQv1wLZyrF5Iq8m\na9Oy4oYrIsEF2uHzCouTKM5n+O4DkwIDAQABo4HuMIHrMB0GA1UdDgQWBBSd/X0L\n/Pq+ZkHvItMtLnxMCAMdhjCBuwYDVR0jBIGzMIGwgBSd/X0L/Pq+ZkHvItMtLnxM\nCAMdhqGBjKSBiTCBhjELMAkGA1UEBhMCQVUxDDAKBgNVBAgTA05TVzEPMA0GA1UE\nBxMGU3lkbmV5MRowGAYDVQQKExFNYWVzdHJhbm8gUHR5IEx0ZDEWMBQGA1UEAxMN\nbWFlc3RyYW5vLmNvbTEkMCIGCSqGSIb3DQEJARYVc3VwcG9ydEBtYWVzdHJhbm8u\nY29tggkA8WlwfatbSnIwDAYDVR0TBAUwAwEB/zANBgkqhkiG9w0BAQUFAAOBgQDE\nhe/18oRh8EqIhOl0bPk6BG49AkjhZZezrRJkCFp4dZxaBjwZTddwo8O5KHwkFGdy\nyLiPV326dtvXoKa9RFJvoJiSTQLEn5mO1NzWYnBMLtrDWojOe6Ltvn3x0HVo/iHh\nJShjAn6ZYX43Tjl1YXDd1H9O+7/VgEWAQQ32v8p5lA==\n-----END CERTIFICATE-----";
                    } else {
                        return "-----BEGIN CERTIFICATE-----\nMIIDezCCAuSgAwIBAgIJAOehBr+YIrhjMA0GCSqGSIb3DQEBBQUAMIGGMQswCQYD\nVQQGEwJBVTEMMAoGA1UECBMDTlNXMQ8wDQYDVQQHEwZTeWRuZXkxGjAYBgNVBAoT\nEU1hZXN0cmFubyBQdHkgTHRkMRYwFAYDVQQDEw1tYWVzdHJhbm8uY29tMSQwIgYJ\nKoZIhvcNAQkBFhVzdXBwb3J0QG1hZXN0cmFuby5jb20wHhcNMTQwMTA0MDUyMjM5\nWhcNMzMxMjMwMDUyMjM5WjCBhjELMAkGA1UEBhMCQVUxDDAKBgNVBAgTA05TVzEP\nMA0GA1UEBxMGU3lkbmV5MRowGAYDVQQKExFNYWVzdHJhbm8gUHR5IEx0ZDEWMBQG\nA1UEAxMNbWFlc3RyYW5vLmNvbTEkMCIGCSqGSIb3DQEJARYVc3VwcG9ydEBtYWVz\ndHJhbm8uY29tMIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDVkIqo5t5Paflu\nP2zbSbzxn29n6HxKnTcsubycLBEs0jkTkdG7seF1LPqnXl8jFM9NGPiBFkiaR15I\n5w482IW6mC7s8T2CbZEL3qqQEAzztEPnxQg0twswyIZWNyuHYzf9fw0AnohBhGu2\n28EZWaezzT2F333FOVGSsTn1+u6tFwIDAQABo4HuMIHrMB0GA1UdDgQWBBSvrNxo\neHDm9nhKnkdpe0lZjYD1GzCBuwYDVR0jBIGzMIGwgBSvrNxoeHDm9nhKnkdpe0lZ\njYD1G6GBjKSBiTCBhjELMAkGA1UEBhMCQVUxDDAKBgNVBAgTA05TVzEPMA0GA1UE\nBxMGU3lkbmV5MRowGAYDVQQKExFNYWVzdHJhbm8gUHR5IEx0ZDEWMBQGA1UEAxMN\nbWFlc3RyYW5vLmNvbTEkMCIGCSqGSIb3DQEJARYVc3VwcG9ydEBtYWVzdHJhbm8u\nY29tggkA56EGv5giuGMwDAYDVR0TBAUwAwEB/zANBgkqhkiG9w0BAQUFAAOBgQCc\nMPgV0CpumKRMulOeZwdpnyLQI/NTr3VVHhDDxxCzcB0zlZ2xyDACGnIG2cQJJxfc\n2GcsFnb0BMw48K6TEhAaV92Q7bt1/TYRvprvhxUNMX2N8PHaYELFG2nWfQ4vqxES\nRkjkjqy+H7vir/MOF3rlFjiv5twAbDKYHXDT7v1YCg==\n-----END CERTIFICATE-----";
                    }
                }
                return _509certificate;
            }

            set { this["x509Certificate"] = value; }
        }

        /// <summary>
        /// Return the IDP url to be used for SSO handshake
        /// </summary>
        /// <returns></returns>
        public string IdpUrl()
        {
            return Idp + "/api/v1/auth/saml";
        }

        /// <summary>
        /// Return the complete SSO init url for this application
        /// </summary>
        /// <returns></returns>
        public string InitUrl()
        {
            return Idm + InitPath;
        }

        /// <summary>
        /// Return the complete SSO consume url for this application
        /// </summary>
        /// <returns></returns>
        public string ConsumeUrl()
        {
            return Idm + ConsumePath;
        }

        /// <summary>
        /// Return the Maestrano logout url to be used for
        /// redirecting a user after logout
        /// </summary>
        /// <returns></returns>
        public string LogoutUrl()
        {
            return Idp + "/app_logout";
        }

        /// <summary>
        /// Return the Maestrano unauthorized page url to
        /// be used when a user is denied access to the application.
        /// Should not need to be used as Maestrano evaluates access
        /// permissions during SSO handshake.
        /// </summary>
        /// <returns></returns>
        public string UnauthorizedUrl()
        {
            return Idp + "/app_access_unauthorized";
        }

        /// <summary>
        /// Build the url used for user session check
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public string SessionCheckUrl(String uid, String session)
        {
            string url = Idp + "/api/v1/auth/saml";
            url += "/" + uid + "?session=" + session;

            return url;
        }

        /// <summary>
        /// Return the SAML Settings to be used by the SAML Request
        /// and Response classes
        /// </summary>
        /// <returns></returns>
        public Saml.Settings SamlSettings()
        {
            Saml.Settings settings = new Saml.Settings();
            settings.AssertionConsumerServiceUrl = ConsumeUrl();
            settings.IdpSsoTargetUrl = IdpUrl();
            settings.IdpCertificate = X509Certificate;
            settings.Issuer = MnoHelper.Api.Id;
            settings.NameIdentifierFormat = NameIdFormat;

            return settings;
        }

        /// <summary>
        /// Build a SAML Request ready to be sent
        /// You can call RedirectUrl() on the returned request
        /// and redirect the user to that url to trigger the Maestrano
        /// SSO handshake
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Saml.Request BuildRequest(NameValueCollection parameters = null)
        {
            return (new Saml.Request(parameters));
        }

        /// <summary>
        /// Build a Saml Response object from a base64 encoded response
        /// </summary>
        /// <param name="samlPostParam">The value of the SAMLResponse POST parameter</param>
        /// <returns></returns>
        public Saml.Response BuildResponse(String samlPostParam)
        {
            var resp = new Saml.Response();
            resp.LoadXmlFromBase64(samlPostParam);

            return resp;
        }

        /// <summary>
        /// Set the maestrano user in session
        /// </summary>
        public void SetSession(HttpSessionState httpSessionObj, User user)
        {
            var mnoSession = new Session(httpSessionObj, user);
            mnoSession.Save();
        }

        /// <summary>
        /// Clear the maestrano session
        /// </summary>
        /// <param name="httpSessionObj"></param>
        public void ClearSession(HttpSessionState httpSessionObj)
        {
            httpSessionObj.Remove("maestrano");
        }
    }
}
