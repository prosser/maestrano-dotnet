using Maestrano.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Maestrano.Sso
{
    public class Group
    {
        public Group()
        {
        }

        /// <summary>
        /// Constructor loading group attributes from a Saml.Response
        /// </summary>
        /// <param name="samlResponse"></param>
        public Group(Saml.Response samlResponse)
        {
            NameValueCollection att = samlResponse.GetAttributes();

            // General info
            Uid = att["group_uid"];
            Name = att["group_name"];
            Email = att["group_email"];
            CompanyName = att["company_name"];
            HasCreditCard = att["group_has_credit_card"].Equals("true");

            // Set Free trial in the past on failure
            try
            {
                FreeTrialEndAt = DateTime.Parse(att["group_end_free_trial"]);
            }
            catch
            {
                FreeTrialEndAt = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            }

            // Geo info
            Currency = att["group_currency"];
            Timezone = TimeZoneConverter.fromOlsonTz(att["group_timezone"]);
            Country = att["group_country"];
            City = att["group_city"];
        }

        public string City { get; set; }

        public string CompanyName { get; set; }

        public string Country { get; set; }

        public string Currency { get; set; }

        public string Email { get; set; }

        public DateTime FreeTrialEndAt { get; set; }

        public bool HasCreditCard { get; set; }

        public string Name { get; set; }

        public TimeZoneInfo Timezone { get; set; }

        public string Uid { get; set; }

        /// <summary>
        /// Return a serializable dictionary describing the resource
        /// </summary>
        /// <returns></returns>
        public JObject ToHash()
        {
            return new JObject(
                new JProperty("provider", "maestrano"),
                new JProperty("uid", Uid),
                new JProperty("info", new JObject(
                    new JProperty("free_trial_end_at", FreeTrialEndAt),
                    new JProperty("company_name", CompanyName),
                    new JProperty("country", Country))),
                new JProperty("extra", null)
             );
        }
    }
}