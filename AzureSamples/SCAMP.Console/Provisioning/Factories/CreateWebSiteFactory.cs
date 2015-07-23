using System;
using Microsoft.Azure.Management.Resources.Models;
using Microsoft.Azure.Management.WebSites;
using Microsoft.Azure.Management.WebSites.Models;
using SCAMP.Provisions;

namespace SCAMP.Provisioning.Factories
{
    class CreateWebSiteFactory : ProvisonFactory<WebSiteManagementClient>
    {
        public WebSite CreateWebSite(WebHostingPlan hostingPlan, ResourceGroupExtended group, Person owner, string siteName = null)
        {
            if (String.IsNullOrEmpty(siteName))
            {
                siteName = "RandomUniqueString";
            }

            var siteParams = new WebSiteCreateOrUpdateParameters(new WebSiteBase()
            {
                Location = this.Location,
                Name = siteName,
                Properties = new WebSiteBaseProperties(hostingPlan.Name)
            });

            return this.Client.WebSites.CreateOrUpdate(group.Name, siteName, null, siteParams).WebSite;
        }
    }
}