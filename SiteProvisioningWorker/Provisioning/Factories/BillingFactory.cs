using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCAMP.Provisioning.Billing;

namespace SCAMP.Provisioning.Factories
{
    class BillingFactory : ProvisonFactory<AzureBillingClient>
    {
        public async Task DoSomething()
        {
            await this.Client.RequestData(DateTimeOffset.Parse("8/1/2015 00:00:00z"), DateTimeOffset.Parse("8/6/2015 00:00:00z"));
        }
    }
}
