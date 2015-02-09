using Maestrano.Api;
using Maestrano.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Maestrano.Account
{
    public class User
    {
        [JsonProperty("id")]
        public String Id {get; set;}

        [JsonProperty("name")]
        public String FirstName { get; set; }

        [JsonProperty("surname")]
        public String LastName { get; set; }

        [JsonProperty("email")]
        public String Email { get; set; }

        [JsonProperty("company_name")]
        public String CompanyName { get; set; }

        [JsonProperty("country")]
        public String Country { get; set; }

        [JsonProperty("sso_session")]
        public String SsoSession { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// The Resource name
        /// </summary>
        /// <returns></returns>
        public static string IndexPath()
        {
            return "account/users";
        }

        /// <summary>
        /// The Single Resource name
        /// </summary>
        /// <returns></returns>
        public static string ResourcePath()
        {
            return IndexPath() + "/{id}";
        }

        /// <summary>
        /// Retrieve all Maestrano users having access to your application
        /// </summary>
        /// <param name="filters">User attributes to filter on</param>
        /// <returns></returns>
        public static List<User> All(NameValueCollection filters = null)
        {
            return MnoClient.All<User>(IndexPath(), filters);
        }

        /// <summary>
        /// Retrieve a single Maestrano user by id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static User Retrieve(string userId)
        {
            return MnoClient.Retrieve<User>(ResourcePath(), userId);
        }

        /// <summary>
        /// Check a user password. Useful if you have specific privileged actions
        /// requiring users to confirm their password.
        /// </summary>
        /// <param name="userIdOrEmail">user id or email address</param>
        /// <param name="password">user password</param>
        /// <returns></returns>
        public static Boolean CheckPassword(string userIdOrEmail, string password)
        {
            var rgx = new Regex(@".*@.*\.(\w+)");
            var att = new NameValueCollection();
            att.Add("password", MnoEncryptor.encrypt(password,MnoHelper.Api.Key));

            // Check if we should match the password against the id or email
            if (rgx.IsMatch(userIdOrEmail)) {
                att.Add("email", userIdOrEmail);
            } else {
                att.Add("id", userIdOrEmail);
            }

            try
            {
                // Raise an error on failure
                MnoClient.Create<User>(IndexPath() + "/authenticate", att);
                return true;
            } catch (Maestrano.Api.ResourceException) {
                return false;
            }
        }
    }
}
