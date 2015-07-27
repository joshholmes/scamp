using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Management.Authorization;
using Microsoft.Azure.Management.Authorization.Models;
using Microsoft.Azure.Management.Resources.Models;
using SCAMP.Provisions;

namespace SCAMP.Provisioning.Factories
{
    class AssignUserToRoleFactory : ProvisonFactory<AuthorizationManagementClient>
    {
        private readonly Lazy<IList<RoleDefinition>> serverRoles;

        private IList<RoleDefinition> ServerRoles
        {
            get
            {
                return serverRoles.Value;
            }
        }

        public AssignUserToRoleFactory()
        {
            serverRoles = new Lazy<IList<RoleDefinition>>(() => base.Client.RoleDefinitions.List().RoleDefinitions.ToList());
        }

        public IEnumerable<RoleAssignment> CreateAndAssignRoleForGroup(ResourceGroupExtended group, Person p, params string[] roles)
        {
            return this.CreateAndAssignRoleForGroup(group, p, (IEnumerable<String>)roles);
        }

        public IEnumerable<RoleAssignment> CreateAndAssignRoleForGroup(ResourceGroupExtended group, Person p, IEnumerable<String> roles)
        {
            var assignments = new List<RoleAssignment>();

            foreach (var roleName in roles)
            {
                var role = ServerRoles.Where(x => String.Equals(x.Properties.RoleName, roleName, StringComparison.OrdinalIgnoreCase)).First();

                var roleCreate = new RoleAssignmentCreateParameters()
                {
                    Properties = new RoleAssignmentProperties()
                    {
                        PrincipalId = p.UserId,
                        RoleDefinitionId = role.Id
                    }
                };

                try
                {

                    var t = this.Client.RoleAssignments.CreateAsync(group.Id, Guid.NewGuid(), roleCreate);
                    t.Wait();
                    assignments.Add(t.Result.RoleAssignment);
                }
                catch (Exception ex)
                {
                    IEnumerable<RoleAssignment> userAssignments = this.Client.RoleAssignments.ListForResourceGroup(group.Name, new ListAssignmentsFilterParameters() { PrincipalId = p.UserId }).RoleAssignments;
                    userAssignments = userAssignments.Where(x => x.Properties.RoleDefinitionId == role.Id);

                    if (userAssignments == null || !userAssignments.Any())
                    {
                        throw;
                    }

                    assignments.AddRange(userAssignments);
                }
            }

            return assignments;
        }
    }
}