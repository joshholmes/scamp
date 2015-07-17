using Microsoft.Azure;
using Microsoft.Azure.Management.Resources;
using Microsoft.Azure.Management.Resources.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.WindowsAzure.Management.WebSites;
using Microsoft.WindowsAzure.Management.WebSites.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SCAMP.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Run().Wait();
        }

        private static async Task Run()
        {
            var managementToken = GetAuthToken();
            var roleToken = new TokenCloudCredentials("325eeaae-8cee-47fe-9a20-2aa220db3435", managementToken);
            var webToken = new Microsoft.WindowsAzure.TokenCloudCredentials("325eeaae-8cee-47fe-9a20-2aa220db3435", managementToken);
            var graphToken = new TokenCloudCredentials("325eeaae-8cee-47fe-9a20-2aa220db3435", GetAuthToken("https://graph.windows.net/"));
            var users = await GetUsers(graphToken, "saintmartincodirectory.onmicrosoft.com");

            var rmClient = new ResourceManagementClient(roleToken);
            var webClient = new WebSiteManagementClient(webToken);
            var factory = new ResourceGroupFactory(rmClient, webClient);
            var term = new TermClass("Testing.2015", "CS150");
            var result = await factory.Create(term, new Person("Eric", "Maino", "emaino@gmail.com", "f132a985-c8b9-4a91-8e0b-0921cce95fed"));

            //var l = rmClient.ResourceGroups.List(new ResourceGroupListParameters());
            //ReadRole(rmClient).Wait();
        }

        private static async Task<IEnumerable<AzureAdUser>> GetUsers(TokenCloudCredentials token, string directory)
        {
            HttpClient client = new HttpClient();
            string uriFormatString = "https://graph.windows.net/{0}/users?api-version=2013-04-05";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, string.Format(uriFormatString, directory));
            request.Headers.Add(HttpRequestHeader.Authorization.ToString(), "Bearer " + token.Token);
            var result = await client.SendAsync(request);
            var content = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<AzureAdDirectoryResult>(content).Value;
        }

        static string GetAuthToken(string resourceUri = "https://management.core.windows.net/")
        {
            var context = new AuthenticationContext("https://login.windows.net/d6763b47-fa3d-4afe-8311-7a062f817f88");
            var result = context.AcquireToken(
                clientId: "855eb965-78e4-4e3b-8d66-93661cfd4d1b",
                resource: resourceUri,
                redirectUri: new Uri("http://localhost/ResourceManagement"),
                promptBehavior: PromptBehavior.Auto
                );

            return result.AccessToken;
        }
    }

    public class AzureAdDirectoryResult
    {
        public IEnumerable<AzureAdUser> Value { get; set; }
    }

    public class AzureAdUser
    {
        public string ObjectId { get; set; }
        public string DisplayName { get; set; }
        public string Mail { get; set; }
        public string UserPrincipalName { get; set; }
        public string GivenName { get; set; }
        public string SurName { get; set; }
    }

    public class RoleAssignmentRequest
    {
        public RoleAssignmentProperties properties { get; set; }
    }

    public class RoleAssignmentProperties
    {
        public string roleDefinitionId { get; set; }
        public string principalId { get; set; }
        public string scope { get; set; }
    }

    public class TermClass
    {
        public TermClass(string term, string name)
        {
            this.Term = term;
            this.Name = name;
        }

        public string Term { get; private set; }
        public string Name { get; private set; }
    }

    public class Person
    {
        public Person(string firstName, string lastName, string microsoftId, string id)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.MicrosoftId = microsoftId;
            this.Id = id;
        }

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string MicrosoftId { get; private set; }
        public string Id { get; set; }
    }

    class RoleCollection
    {
        public IEnumerable<Role> Value { get; set; }
    }

    class Role
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public RoleProperties Properties { get; set; }
    }

    class RoleProperties
    {
        public string RoleName { get; set; }
    }
    public class ResourceGroupFactory
    {
        private readonly ResourceManagementClient client;
        private readonly WebSiteManagementClient websiteClient;
        private readonly Regex safeNameRegex;
        private readonly ResourceGroup defaultLocation;

        public ResourceGroupFactory(ResourceManagementClient client, WebSiteManagementClient webClient)
        {
            this.client = client;
            this.websiteClient = webClient;
            this.safeNameRegex = new Regex(@"[^a-z0-9\.]", RegexOptions.IgnoreCase);
            this.defaultLocation = new ResourceGroup("West US");
        }

        public async Task<ResourceGroupExtended> Create(TermClass termClass, Person resourceOwner)
        {
            //string groupName = String.Join(".", termClass.Term, termClass.Name, resourceOwner.LastName, resourceOwner.FirstName, UniqueSuffix);
            //groupName = safeNameRegex.Replace(groupName, String.Empty);
            //var group = (await client.ResourceGroups.CreateOrUpdateAsync(groupName, defaultLocation)).ResourceGroup;
            //await AddUserToRole(group.Name, "Contributor", resourceOwner.Id);
            await CreateWebSite(null);
            return null;
        }

        private async Task CreateWebSite(ResourceGroupExtended group)
        {
            var spaces = await websiteClient.WebSpaces.ListAsync(new CancellationToken());

            //var siteName = "EricMai.Test" //group.Name.Replace(".", "-");
            //var result = await websiteClient.WebSites.CreateAsync(siteName, new WebSiteCreateParameters()
            //{
            //    Name = siteName,
            //    ServerFarm = 
            //}, new CancellationToken());
        }

        private async Task AddUserToRole(string groupName, string roleName, string userId)
        {
            var role = (await ReadRoles(client)).Value.Where(x => String.Equals(x.Properties.RoleName, roleName, StringComparison.OrdinalIgnoreCase)).Single();
            var roleRequest = new RoleAssignmentRequest()
            {
                properties = new RoleAssignmentProperties()
                {
                    roleDefinitionId = role.Id,
                    scope = String.Format(@"/subscriptions/{0}/resourceGroups/{1}/", client.Credentials.SubscriptionId, groupName),
                    principalId = userId
                },
            };

            var s = String.Format("{0}{1}providers/Microsoft.Authorization/roleAssignments/{2}?api-version={3}", client.BaseUri.AbsoluteUri.TrimEnd(new[] { '/' }), roleRequest.properties.scope, roleRequest.properties.principalId, client.ApiVersion);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, s);
            request.Headers.Add(HttpRequestHeader.Authorization.ToString(), "Bearer " + ((TokenCloudCredentials)client.Credentials).Token);
            var json = JsonConvert.SerializeObject(roleRequest);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            var content = await (await client.HttpClient.SendAsync(request)).Content.ReadAsStringAsync();
        }

        private static async Task<RoleCollection> ReadRoles(ResourceManagementClient c)
        {
            var s = String.Format("{0}subscriptions/{1}/providers/Microsoft.Authorization/roleDefinitions?api-version={2}", c.BaseUri.AbsoluteUri, c.Credentials.SubscriptionId, c.ApiVersion);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, s);
            request.Headers.Add(HttpRequestHeader.Authorization.ToString(), "Bearer " + ((TokenCloudCredentials)c.Credentials).Token);
            var content = await (await c.HttpClient.SendAsync(request)).Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<RoleCollection>(content);
        }

        public object UniqueSuffix
        {
            get
            {
                return Path.GetFileName(Path.GetRandomFileName());
            }
        }
    }
}
