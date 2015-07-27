using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using SCAMP.Provisioning;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SiteProvisioningWorker
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.TraceInformation("SiteProvisioningWorker is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            bool result = base.OnStart();

            Trace.TraceInformation("SiteProvisioningWorker has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("SiteProvisioningWorker is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("SiteProvisioningWorker has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            SubscriptionClient client = GetOrRenewClient();
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var message = await client.ReceiveAsync();

                    if (message == null)
                    {
                        Trace.TraceInformation("No Messages");
                    }
                    else
                    {
                        ProvisionSite(message.GetBody<ProvisionRequest>());
                        await message.CompleteAsync();
                        Trace.TraceInformation("Working");
                    }
                }
                catch (TimeoutException)
                {
                    Trace.TraceInformation("Connection timeout");
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                    client = GetOrRenewClient();
                }

                await Task.Delay(1000);
            }
        }

        private SubscriptionClient GetOrRenewClient()
        {
            var conn = CloudConfigurationManager.GetSetting("SBConnectionString");
            var topic = CloudConfigurationManager.GetSetting("SBTopic");
            var subscription = CloudConfigurationManager.GetSetting("SBSubscription");
            return SubscriptionClient.CreateFromConnectionString(conn, topic, subscription);
        }

        private void ProvisionSite(ProvisionRequest request)
        {
            var storageKey = CloudConfigurationManager.GetSetting("StorageConnectionString");
            var tableName = CloudConfigurationManager.GetSetting("ProvisionTable");
            var azure = CloudStorageAccount.Parse(storageKey);
            var storage = new ProvisionedSiteStorage(azure.CreateCloudTableClient().GetTableReference(tableName));
            var p = new SiteProvisioner(storage);
            p.Provision(request);
        }
    }
}
