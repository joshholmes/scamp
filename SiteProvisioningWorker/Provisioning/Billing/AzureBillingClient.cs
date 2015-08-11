using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Hyak.Common;
using Microsoft.Azure;
using Newtonsoft.Json;

namespace SCAMP.Provisioning.Billing
{
    public interface IAzureBillingClient : IDisposable
    {
        string ApiVersion { get; }
        Uri BaseUri { get; }
        SubscriptionCloudCredentials Credentials { get; }
    }

    class AzureBillingClient : ServiceClient<AzureBillingClient>, IAzureBillingClient
    {
        public AzureBillingClient(SubscriptionCloudCredentials credentials, Uri baseUri)
        {
            this.ApiVersion = "2015-06-01-preview";
            this.Credentials = credentials;
            this.BaseUri = baseUri;
            credentials.InitializeServiceClient(this);
        }

        public string ApiVersion {get; private set;}

        public Uri BaseUri { get; private set; }

        public SubscriptionCloudCredentials Credentials { get; private set; }

        public async Task RequestData(DateTimeOffset startTime, DateTimeOffset endTime)
        {
            string baseService = String.Format("{0}/subscriptions/{1}/providers/Microsoft.Commerce/UsageAggregates",
                this.BaseUri.AbsoluteUri.TrimEnd('/'),
                this.Credentials.SubscriptionId);

            var uri = GenerateUri(baseService, new Dictionary<String,Object>()
            {
                {"api-version", this.ApiVersion},
                {"reportedStartTime","2015-08-05T00:00:00+00:00"},
                {"reportedEndTime", "2015-08-05T01:00:00+00:00"},
                {"aggregationGranularity","hourly"},
                {"showDetails",true}
            });
            
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            await this.Credentials.ProcessHttpRequestAsync(request, new System.Threading.CancellationToken());
            var result = await this.HttpClient.SendAsync(request);
            var json = await result.Content.ReadAsStringAsync();
            var y = JsonConvert.DeserializeObject<AzureBillingResult>(json);
        }

        private Uri GenerateUri(string uri, IDictionary<String,object> query)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(uri);

            if (query != null && query.Any())
            {
                sb.Append("?");

                bool append = false;

                foreach (var key in query.Keys)
                {
                    if (append)
                    {
                        sb.Append("&");
                    }

                    sb.AppendFormat("{0}={1}", key, Uri.EscapeDataString(query[key].ToString()));
                    append = true;
                }
            }

            return new Uri(sb.ToString());
        }

        class AzureBillingResult
        {
            public Result[] Value { get; set; }
        }
        class Result
        {
            public MeterResult Properties { get; set; }
        }

        class MeterResult
        {
            public DateTimeOffset UsageStartTime { get; set; }
            public DateTimeOffset UsageEndTime { get; set; }
            public string MeterName { get; set; }
            public string MeterRegion { get; set; }
            public string MeterCategory { get; set; }
            public string Unit { get; set; }
            public MeterResultFields InfoFields { get; set; }
            public Double Quantity { get; set; }
        }

        class MeterResultFields
        {
            public string MeteredRegion { get; set; }
            public string MeteredService { get; set; }
            public string Project { get; set; }
            public string MeteredServiceType { get; set; }
        }
    }
}
