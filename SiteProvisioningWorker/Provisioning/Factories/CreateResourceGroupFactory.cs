using Microsoft.Azure.Management.Resources;
using Microsoft.Azure.Management.Resources.Models;

namespace SCAMP.Provisioning.Factories
{
    class CreateResourceGroupFactory : ProvisonFactory<ResourceManagementClient>
    {
        public ResourceGroupExtended CreateGroup(string groupName)
        {
            var resourceGroups = this.Client.ResourceGroups;

            var groupConfig = new ResourceGroup()
            {
                Location = this.Location,
            };

            return resourceGroups.CreateOrUpdate(groupName, groupConfig).ResourceGroup;
        }
    }
}