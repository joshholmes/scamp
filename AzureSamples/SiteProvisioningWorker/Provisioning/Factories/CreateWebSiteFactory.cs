using System;
using Microsoft.Azure.Management.Resources.Models;
using Microsoft.Azure.Management.WebSites;
using Microsoft.Azure.Management.WebSites.Models;
using SCAMP.Provisions;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SCAMP.Provisioning.Factories
{
    class CreateWebSiteFactory : ProvisonFactory<WebSiteManagementClient>
    {
        public WebSite CreateWebSite(WebHostingPlan hostingPlan, ResourceGroupExtended group, Person owner, string siteName = null)
        {
            if (String.IsNullOrEmpty(siteName))
            {
                siteName = Regex.Replace(Path.GetRandomFileName(), @"[^a-z0-9]", String.Empty, RegexOptions.IgnoreCase);
            }

            var siteParams = new WebSiteCreateOrUpdateParameters(new WebSiteBase()
            {
                Location = this.Location,
                Name = siteName,
                Properties = new WebSiteBaseProperties(hostingPlan.Name)
            });

            return this.Client.WebSites.CreateOrUpdate(group.Name, siteName, null, siteParams).WebSite;
        }

        public WebSiteGetPublishProfileResponse.PublishProfile GetPublishingProfile(WebSite site, ResourceGroupExtended group)
        {
            var profile = this.Client.WebSites.GetPublishProfile(group.Name, site.Name, null).Single(x => String.Equals(x.PublishMethod, "FTP"));
            return profile;
        }
    }
}