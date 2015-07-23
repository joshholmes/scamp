using System;
using System.Linq;
using System.Text.RegularExpressions;
using SCAMP.Provisioning.Factories;
using SCAMP.Provisions;

namespace SCAMP.Provisioning
{
    class TestProvision
    {
        public void Provision()
        {
            // Need to figure this one out..
            var accountId = "182dd5b9-4dc7-4cff-b518-282874c84784";
            var subscriptionId = Guid.Parse("325eeaae-8cee-47fe-9a20-2aa220db3435");
            var servicePlanName = "Student-Web-Site";

            var student = new Person()
            {
                FirstName = "Eric",
                LastName = "Maino",
                MicrosoftId = "passport@meeteric.net",
                UserId = Guid.NewGuid()
            };

            var professor = new Person()
            {
                FirstName = "Professor",
                LastName = "Last Name",
                MicrosoftId = "emaino@gmail.com",
                UserId = Guid.Parse("f132a985-c8b9-4a91-8e0b-0921cce95fed")
            };


            var authToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ik1uQ19WWmNBVGZNNXBPWWlKSE1iYTlnb0VLWSIsImtpZCI6Ik1uQ19WWmNBVGZNNXBPWWlKSE1iYTlnb0VLWSJ9.eyJhdWQiOiJodHRwczovL21hbmFnZW1lbnQuY29yZS53aW5kb3dzLm5ldC8iLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC9kNjc2M2I0Ny1mYTNkLTRhZmUtODMxMS03YTA2MmY4MTdmODgvIiwiaWF0IjoxNDM3NjA5OTY1LCJuYmYiOjE0Mzc2MDk5NjUsImV4cCI6MTQzNzYxMzg2NSwidmVyIjoiMS4wIiwidGlkIjoiZDY3NjNiNDctZmEzZC00YWZlLTgzMTEtN2EwNjJmODE3Zjg4Iiwib2lkIjoiNWZlZDUxODgtZjE0ZC00OGRlLTk1YzgtNjA5MGJhZWQwNjU1IiwiZW1haWwiOiJlcmljLm1haW5vQGhvdG1haWwuY29tIiwicHVpZCI6IjEwMDNCRkZEODU0QzlBRjAiLCJpZHAiOiJsaXZlLmNvbSIsImFsdHNlY2lkIjoiMTpsaXZlLmNvbTowMDA2NDAwMDkyMDVDRUIyIiwic3ViIjoibXRkUldvUklNQ3RlN1NXZFctTThNcTBHRHgxaFBYQmhlTThyNHl3T3V4MCIsImdpdmVuX25hbWUiOiJFcmljIiwiZmFtaWx5X25hbWUiOiJNYWlubyIsIm5hbWUiOiJFcmljIE1haW5vIiwiYW1yIjpbInB3ZCJdLCJncm91cHMiOlsiYTNlNjMxODctNDc0ZC00ZWM1LWIyMDItZjcwOTFiMzBjMTQ5Il0sInVuaXF1ZV9uYW1lIjoibGl2ZS5jb20jZXJpYy5tYWlub0Bob3RtYWlsLmNvbSIsIndpZHMiOlsiNjJlOTAzOTQtNjlmNS00MjM3LTkxOTAtMDEyMTc3MTQ1ZTEwIl0sImFwcGlkIjoiODU1ZWI5NjUtNzhlNC00ZTNiLThkNjYtOTM2NjFjZmQ0ZDFiIiwiYXBwaWRhY3IiOiIwIiwic2NwIjoidXNlcl9pbXBlcnNvbmF0aW9uIiwiYWNyIjoiMSJ9.txpNE1cYFwae-OK1tR4yr4PdNqdhC8vO9BO1zxLqNmCVAc-t9102LtmcIgCmJdwKDmecXY6T4TRs7g3GVUmKa5a6Zp3bdcTn4Hk8FPR2qMOrRagIxn5538T8iJ_UgaSNYvstmnivCCeUrCIg3onJuajr_ZtqCiMALcfCSp7lkS02hVez8_St-TV7iVWvoLKKkyJkGj-aSSNf9kejbWUGtXdTA-3MpqBbFidWQrDJ1iEdKHaJqsr0Hu3r6Hv3MJTW0JetUEyMN22ssMnAAQJaauTMpN5qIUDxt2XXLIFzbLqecjhINWUlbHkY1RMs2DJaLDWrgTTPqYWLbUim0_x56Q";

            Regex alphaNumericWithPeriod = new Regex(@"[^a-z0-9\.-]", RegexOptions.IgnoreCase);
            var groupName = String.Join(".", "Testing", student.LastName, student.FirstName, student.MicrosoftId.Split('@').First());
            groupName = alphaNumericWithPeriod.Replace(groupName, String.Empty);

            var factory = new AzureFactory(subscriptionId, accountId, authToken);
            var group = factory.Create<CreateResourceGroupFactory>().CreateGroup(groupName);
            var plan = factory.Create<CreateWebHostingPlanFactory>().CreateWebPlan(group, servicePlanName);
            factory.Create<AssignUserToRoleFactory>().CreateAndAssignRoleForGroup(group, professor, "WebSite Contributor");
            var site = factory.Create<CreateWebSiteFactory>().CreateWebSite(plan, group, professor);
        }
    }
}