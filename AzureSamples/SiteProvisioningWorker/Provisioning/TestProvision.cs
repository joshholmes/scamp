using Microsoft.Azure.Management.WebSites.Models;
using Microsoft.WindowsAzure.Storage.Table;
using SCAMP.Provisioning.Factories;
using SCAMP.Provisions;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace SCAMP.Provisioning
{
    public class SiteProvisioner
    {
        private readonly ProvisionedSiteStorage storage;

        public SiteProvisioner(ProvisionedSiteStorage storage)
        {
            this.storage = storage;
        }

        public void Provision(ProvisionRequest request)
        {
            var subscriptionId = request.SubscriptionId;
            var accountId = request.AccountId;
            var servicePlanName = request.ServicePlanName;
            var authToken = request.AuthorizationToken;
            var professor = request.Professor;

            foreach (var student in request.Students)
            {
                Regex alphaNumericWithPeriod = new Regex(@"[^a-z0-9\.-]", RegexOptions.IgnoreCase);
                var groupName = String.Join(".", request.GroupPrefix, student.LastName, student.FirstName, student.MicrosoftId.Split('@').First());
                groupName = alphaNumericWithPeriod.Replace(groupName, String.Empty);

                var factory = new AzureFactory(subscriptionId, accountId, authToken);
                var group = factory.Create<CreateResourceGroupFactory>().CreateGroup(groupName);
                var plan = factory.Create<CreateWebHostingPlanFactory>().CreateWebPlan(group, servicePlanName);
                factory.Create<AssignUserToRoleFactory>().CreateAndAssignRoleForGroup(group, professor, "WebSite Contributor");
                var webFactory = factory.Create<CreateWebSiteFactory>();
                var site = webFactory.CreateWebSite(plan, group, professor);
                var publishProfile = webFactory.GetPublishingProfile(site, group);
                storage.Save(student, site, publishProfile);
            }

        }
    }
    
    public class ProvisionedSiteStorage
    {
        private readonly CloudTable table;

        public ProvisionedSiteStorage(CloudTable siteTable)
        {
            this.table = siteTable;
            table.CreateIfNotExists();
        }

        internal void Save(Person student, WebSite site, WebSiteGetPublishProfileResponse.PublishProfile publishProfile)
        {
            table.Execute(TableOperation.Insert(new ProvisionedSiteEntity(student, site, publishProfile)));
        }

        private class ProvisionedSiteEntity : TableEntity
        {
            public ProvisionedSiteEntity(Person student, WebSite site, WebSiteGetPublishProfileResponse.PublishProfile publishProfile)
            {
                this.PartitionKey = "ProvisionedSite";
                this.RowKey = student.MicrosoftId;
                this.FirstName = student.FirstName;
                this.LastName = student.LastName;
                this.WebSite = site.Properties.HostNames.First();
                this.FtpSite = publishProfile.PublishUrl;
                this.FtpUserName = publishProfile.UserName;
                this.FtpPassword = publishProfile.UserPassword;
            }

            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string WebSite { get; set; }
            public string FtpSite { get; set; }
            public string FtpUserName { get; set; }
            public string FtpPassword { get; set; }
        }
    }
}