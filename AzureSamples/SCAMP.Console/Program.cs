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

namespace ConsoleApplication9
{
    static class Program
    {
        private const string clientIdFromAzureAdApplication = "855eb965-78e4-4e3b-8d66-93661cfd4d1b";
        private const string clientAppRedirectUri = "http://localhost/ResourceManagement";
        private const string authenticationUrl = "https://login.windows.net/d6763b47-fa3d-4afe-8311-7a062f817f88";
        private const string subscriptionId = "325eeaae-8cee-47fe-9a20-2aa220db3435";
        private const string azureManagementResourceUri = "https://management.core.windows.net/";
        private static string[] roles = new string[] {
            "WebSite Contributor",
            "Storage Account Contributor",
            "Sql Server Contributor"
        };

        private static Regex alphaNumeric = new Regex("[^a-z0-9]", RegexOptions.IgnoreCase);
        private static Regex alphaNumericWithPeriod = new Regex(@"[^a-z0-9\.-]", RegexOptions.IgnoreCase);

        static void Main(string[] args)
        {
            var azureProfile = new AzureProfile();
            var profileClient = new ProfileClient(azureProfile);
            var azureEnvironment = AzureEnvironment.PublicEnvironments["AzureCloud"];

            var token = GetAuthToken();
            profileClient.InitializeProfile(azureEnvironment, Guid.Parse(subscriptionId), token, "182dd5b9-4dc7-4cff-b518-282874c84784", "");

            var webClient = azureProfile.CreateClient<WebSiteManagementClient>();
            var rmClient = azureProfile.CreateClient<ResourceManagementClient>();
            var authClient = azureProfile.CreateClient<AuthorizationManagementClient>();
            var creds = AzureSession.AuthenticationFactory.GetSubscriptionCloudCredentials(azureProfile.Context);
            var graphClient = new GraphRbacManagementClient("d6763b47-fa3d-4afe-8311-7a062f817f88", creds, azureProfile.Context.Environment.GetEndpointAsUri(AzureEnvironment.Endpoint.Graph));

            var profile = Create(rmClient, authClient, webClient, "Eric", "Maino", "emaino@gmail.com");
        }

        public static TClient CreateClient<TClient>(this AzureProfile profile) where TClient : Hyak.Common.ServiceClient<TClient>
        {
            return AzureSession.ClientFactory.CreateClient<TClient>(profile.Context, AzureEnvironment.Endpoint.ResourceManager);
        }

        public static WebSiteGetPublishProfileResponse.PublishProfile Create(IResourceManagementClient client, IAuthorizationManagementClient auth, WebSiteManagementClient web, string firstName, string lastName, string microsoftId)
        {
            string namePrefix = "Testing.2015";
            string servicePlan = "Student-Web-Site";
            string uniqueKey = alphaNumeric.Replace(Path.GetRandomFileName(), "");
            string groupName = alphaNumericWithPeriod.Replace(String.Join(".", namePrefix, lastName, firstName, uniqueKey), "");

            var resourceGroups = client.ResourceGroups;
            var groupConfig = new ResourceGroup()
            {
                Location = "West US",
            };

            //var user = graph.User.Get(microsoftId).User;
            var group = resourceGroups.CreateOrUpdate(groupName, groupConfig);
            var adRoles = auth.RoleDefinitions.List().RoleDefinitions.ToList();

            foreach (var roleName in roles)
            {
                var role = adRoles.Where(x => String.Equals(x.Properties.RoleName, roleName, StringComparison.OrdinalIgnoreCase)).First();

                var roleCreate = new RoleAssignmentCreateParameters()
                {
                    Properties = new RoleAssignmentProperties()
                    {
                        PrincipalId = Guid.Parse("f132a985-c8b9-4a91-8e0b-0921cce95fed"),
                        RoleDefinitionId = role.Id
                    }
                };

                auth.RoleAssignments.Create(group.ResourceGroup.Id, Guid.NewGuid(), roleCreate);
            }

            var plan = new WebHostingPlanCreateOrUpdateParameters(
                new WebHostingPlan()
                {
                    Location = "West US",
                    Name = servicePlan,
                    Properties = new WebHostingPlanProperties()
                    {
                        Sku = SkuOptions.Shared,
                        NumberOfWorkers = 1,
                        WorkerSize = WorkerSizeOptions.Small,
                    }
                }
            );
            var hostingPlan = web.WebHostingPlans.CreateOrUpdate(groupName, plan);

            var siteParams = new WebSiteCreateOrUpdateParameters(new WebSiteBase()
            {
                Location = plan.WebHostingPlan.Location,
                Name = uniqueKey,
                Properties = new WebSiteBaseProperties(servicePlan)
            });
            var site = web.WebSites.CreateOrUpdate(groupName, uniqueKey, null, siteParams);

            return web.WebSites.GetPublishProfile(groupName, uniqueKey, null).Single(x => String.Equals(x.PublishMethod, "FTP"));
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
