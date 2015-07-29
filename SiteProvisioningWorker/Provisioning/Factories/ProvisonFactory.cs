using Microsoft.Azure.Common.Authentication;
using Microsoft.Azure.Common.Authentication.Models;

namespace SCAMP.Provisioning.Factories
{
    abstract class ProvisonFactory<TClient> : IProvisionAction
        where TClient : Hyak.Common.ServiceClient<TClient>
    {
        public void Initialize(AzureProfile profile, string location)
        {
            this.Client = AzureSession.ClientFactory.CreateClient<TClient>(profile.Context, AzureEnvironment.Endpoint.ResourceManager);
            this.Location = location;
        }

        protected TClient Client { get; private set; }
        protected string Location { get; private set; }
    }
}