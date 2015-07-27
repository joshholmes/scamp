using System;
using Microsoft.Azure.Common.Authentication;
using Microsoft.Azure.Common.Authentication.Models;

namespace SCAMP.Provisioning
{
    class AzureFactory
    {
        private readonly string location;
        private readonly AzureProfile profile;

        public AzureFactory(Guid subscriptionId, string accountId, string authorizationToken)
        {
            location = "West US";
            profile = new AzureProfile();
            var profileClient = new ProfileClient(profile);
            var productionCloud = AzureEnvironment.PublicEnvironments["AzureCloud"];
            profileClient.InitializeProfile(productionCloud, subscriptionId, authorizationToken, accountId, null);
        }

        public T Create<T>() where T : IProvisionAction, new()
        {
            T action = new T();
            action.Initialize(profile, location);
            return action;
        }
    }
}
