using SCAMP.Azure;
using SCAMP.Contracts;
using SCAMP.Provisioning.Factories;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SCAMP.Provisioning
{
    public class SiteProvisioner
    {
        private readonly IScampContext context;

        public SiteProvisioner(IScampContext context)
        {
            this.context = context;
        }

        public async Task Provision(ProvisionRequest request)
        {
            await Task.Yield();

            var subscriptionId = request.SubscriptionId;
            var accountId = request.AccountId;
            var authToken = request.AuthorizationToken;

            bool failure = false;
            request.Course.State = CourseState.Provisioning;
            context.UpdateCourse(request.Course);

            try
            {
                Regex alphaNumericWithPeriod = new Regex(@"[^a-z0-9\.-]", RegexOptions.IgnoreCase);
                //var groupName = String.Join(".", request.Course.Name, student.LastName, student.FirstName, student.MicrosoftId.Split('@').First());
                var groupName = String.Join(".", request.Course.Name, request.Course.Id.ToString());
                groupName = alphaNumericWithPeriod.Replace(groupName, String.Empty);

                var factory = new AzureFactory(subscriptionId, accountId.ToString(), authToken);
                var group = factory.Create<CreateResourceGroupFactory>().CreateGroup(groupName);
                var plan = factory.Create<CreateWebHostingPlanFactory>().CreateWebPlan(group, "Student-Web-Site");
                factory.Create<AssignUserToRoleFactory>().CreateAndAssignRoleForGroup(group, request.PrincipalOwnerId, "WebSite Contributor");

                foreach (var student in context.GetStudentsInCourse(request.Course))
                {
                    var webFactory = factory.Create<CreateWebSiteFactory>();
                    var site = webFactory.CreateWebSite(plan, group);
                    var publishProfile = webFactory.GetPublishingProfile(site, group);
                    context.AddWebSite(request.Course, student, new Models.WebSite()
                    {
                        WebSiteUri = new Uri("http://" + site.Properties.HostNames.First()),
                        FtpUri = new Uri(publishProfile.PublishUrl),
                        FtpUsername = publishProfile.UserName,
                        FtpPassword = publishProfile.UserPassword
                    });
                }
            }
            catch
            {
                failure = true;
                request.Course.State = CourseState.Failed;
                throw;
            }
            finally
            {
                if (!failure)
                {
                    request.Course.State = CourseState.Complete;
                }

                context.UpdateCourse(request.Course);
            }

        }
    }
}