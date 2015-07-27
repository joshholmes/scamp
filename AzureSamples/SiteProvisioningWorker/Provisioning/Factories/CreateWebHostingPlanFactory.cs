using System.Net;
using Microsoft.Azure.Management.Resources.Models;
using Microsoft.Azure.Management.WebSites;
using Microsoft.Azure.Management.WebSites.Models;

namespace SCAMP.Provisioning.Factories
{
    class CreateWebHostingPlanFactory : ProvisonFactory<WebSiteManagementClient>
    {
        public WebHostingPlan CreateWebPlan(ResourceGroupExtended group, string servicePlanName)
        {
            var hostingPlan = new WebHostingPlan()
            {
                Location = this.Location,
                Name = servicePlanName,
                Properties = new WebHostingPlanProperties()
                {
                    Sku = SkuOptions.Shared,
                    NumberOfWorkers = 1,
                    WorkerSize = WorkerSizeOptions.Small,
                }
            };
            
            var plan = new WebHostingPlanCreateOrUpdateParameters(hostingPlan);
            var result = this.Client.WebHostingPlans.CreateOrUpdate(group.Name, plan);

            if (result.StatusCode != HttpStatusCode.OK)
            {
                throw null;
            }
            else if (result.WebHostingPlan != null)
            {
                hostingPlan = result.WebHostingPlan;
            }

            return hostingPlan;
        }
    }
}