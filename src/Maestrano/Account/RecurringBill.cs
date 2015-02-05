using Maestrano.Api;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Maestrano.Account
{
    public class RecurringBill
    {
        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("cycles")]
        public Int16? Cycles { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("frequency")]
        public Int16? Frequency { get; set; }

        [JsonProperty("group_id")]
        public string GroupId { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("initial_cents")]
        public Int32? InitialCents { get; set; }

        [JsonProperty("period")]
        public String Period { get; set; }

        // Mandatory for creation
        [JsonProperty("price_cents")]
        public Int32 PriceCents { get; set; }

        [JsonProperty("start_date")]
        public DateTime? StartDate { get; set; }

        [JsonProperty("status")]
        public RecurringBillStatus Status { get; set; }

        [JsonProperty("units")]
        public decimal? Units { get; set; }

        [JsonProperty("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        public static List<RecurringBill> All(NameValueCollection filters = null)
        {
            return MnoClient.All<RecurringBill>(IndexPath(), filters);
        }

        public static RecurringBill Create(String groupId, Int32 priceCents, String description, String currency = "AUD", Int32 initialCents = 0, Int16? frequency = null, Int16? cycles = null, DateTime? startDate = null)
        {
            var att = new NameValueCollection();
            att.Add("groupId", groupId);
            att.Add("priceCents", priceCents.ToString());
            att.Add("description", description);
            att.Add("currency", currency);
            att.Add("initialCents", initialCents.ToString());
            if (frequency.HasValue)
                att.Add("frequency", frequency.Value.ToString());
            if (cycles.HasValue)
                att.Add("cycles", cycles.Value.ToString());
            if (startDate.HasValue)
                att.Add("startDate", startDate.Value.ToString("s"));

            return MnoClient.Create<RecurringBill>(IndexPath(), att);
        }

        /// <summary>
        /// The Resource name
        /// </summary>
        /// <returns></returns>
        public static string IndexPath()
        {
            return "account/recurring_bills";
        }

        /// <summary>
        /// The Single Resource name
        /// </summary>
        /// <returns></returns>
        public static string ResourcePath()
        {
            return IndexPath() + "/{id}";
        }

        public static RecurringBill Retrieve(string billId)
        {
            return MnoClient.Retrieve<RecurringBill>(ResourcePath(), billId);
        }

        public Boolean Cancel()
        {
            if (Id != null)
            {
                RecurringBill respBill = MnoClient.Delete<RecurringBill>(ResourcePath(), Id);
                Status = respBill.Status;
                UpdatedAt = respBill.UpdatedAt;
                return (Status.Equals("cancelled"));
            }

            return false;
        }
    }
}