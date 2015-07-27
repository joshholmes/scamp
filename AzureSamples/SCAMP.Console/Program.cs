using Microsoft.Azure.Common.Authentication;
using Microsoft.Azure.Common.Authentication.Models;
using Microsoft.Azure.Graph.RBAC;
using Microsoft.Azure.Management.Authorization;
using Microsoft.Azure.Management.Authorization.Models;
using Microsoft.Azure.Management.Resources;
using Microsoft.Azure.Management.Resources.Models;
using Microsoft.Azure.Management.WebSites;
using Microsoft.Azure.Management.WebSites.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SCAMP.Provisioning;
using SCAMP.Provisions;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using System.Text;
using SCAMP;
using Microsoft.ServiceBus.Messaging;

namespace ConsoleApplication9
{
    static class Program
    {
        private const string clientIdFromAzureAdApplication = "855eb965-78e4-4e3b-8d66-93661cfd4d1b";
        private const string clientAppRedirectUri = "http://localhost/ResourceManagement";
        private const string authenticationUrl = "https://login.windows.net/d6763b47-fa3d-4afe-8311-7a062f817f88";
        private const string azureManagementResourceUri = "https://management.core.windows.net/";

        static void Main(string[] args)
        {
            var token = GetAuthToken();
            var request = new ProvisionRequest()
            {
                AccountId = "182dd5b9-4dc7-4cff-b518-282874c84784",
                SubscriptionId = Guid.Parse("325eeaae-8cee-47fe-9a20-2aa220db3435"),
                ServicePlanName = "Student-Web-Site",
                GroupPrefix = "Testing",
                AuthorizationToken = token,
                Professor = new Person()
                {
                    FirstName = "Professor",
                    LastName = "Last Name",
                    MicrosoftId = "emaino@gmail.com",
                    UserId = Guid.Parse("f132a985-c8b9-4a91-8e0b-0921cce95fed")
                },
                Students = new List<Person>()
                {
                    new Person()
                    {
                        FirstName = "Eric",
                        LastName = "Maino",
                        MicrosoftId = "passport@meeteric.net",
                        UserId = Guid.NewGuid()
                    }
                }
            };

            TopicClient client = TopicClient.CreateFromConnectionString("Endpoint=sb://ericmaino.servicebus.windows.net/;SharedAccessKeyName=Publish;SharedAccessKey=WncmI9nQYedsBTeRzQpfgMsIpXLsvafceWycgiZUbow=", "provisionrequests");
            client.Send(new BrokeredMessage(request));

            //var azureStorage = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            //var storage = new ProvisionedSiteStorage(azureStorage.CreateCloudTableClient().GetTableReference("ProvisionedSites"));
            //var provisioner = new SiteProvisioner(storage);
            //provisioner.Provision(request);
        }

        static string GetAuthToken(string resourceUri = azureManagementResourceUri)
        {
            var context = new AuthenticationContext(authenticationUrl);
            var result = context.AcquireToken(
                clientId: clientIdFromAzureAdApplication,
                resource: resourceUri,
                redirectUri: new Uri(clientAppRedirectUri),
                promptBehavior: PromptBehavior.Auto
                );

            return result.AccessToken;
        }
    }
}
