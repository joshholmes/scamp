using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using SCAMP.Azure;
using SCAMP.Contracts;
using SCAMP.Provisioning;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using SCAMP.Provisioning.Factories;

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
                    var message = await client.ReceiveAsync(TimeSpan.FromSeconds(15));

                    if (message != null)
                    {
                        await ProvisionSite(message.GetBody<ProvisionRequest>());
                    }
                }
                catch (TimeoutException)
                {
                    Trace.TraceInformation("Connection timeout");
                }
                catch (Exception ex)
                {
                    LogException(ex);
                    client = GetOrRenewClient();
                }

                await Task.Delay(1000);
            }
        }

        private void LogException(Exception ex)
        {
            AggregateException agg = ex as AggregateException;

            if (agg != null)
            {
                foreach (var innerEx in agg.InnerExceptions)
                {
                    Trace.TraceError(innerEx.ToString());
                }
            }

            Trace.TraceError(ex.ToString());
        }

        private SubscriptionClient GetOrRenewClient()
        {
            var conn = CloudConfigurationManager.GetSetting("SBConnectionString");
            var topic = CloudConfigurationManager.GetSetting("SBTopic");
            var subscription = CloudConfigurationManager.GetSetting("SBSubscription");

            var factory = MessagingFactory.CreateFromConnectionString(conn);
            return factory.CreateSubscriptionClient(topic, subscription, ReceiveMode.ReceiveAndDelete);
        }

        private async Task ProvisionSite(ProvisionRequest request)
        {
            var storage = new ScampAzureContext();
            var p = new SiteProvisioner(storage);
            await p.Provision(request);
        }
    }
}
