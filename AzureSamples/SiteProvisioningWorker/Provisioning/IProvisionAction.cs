using Microsoft.Azure.Common.Authentication.Models;

namespace SCAMP.Provisioning
{
    interface IProvisionAction
    {
        void Initialize(AzureProfile profile, string location);
    }
}