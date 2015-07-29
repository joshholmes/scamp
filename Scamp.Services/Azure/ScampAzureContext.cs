using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using SCAMP.Contracts;
using SCAMP.Models;

namespace SCAMP.Azure
{
    public class ScampAzureContext : IScampContext
    {
        private readonly HttpClient client;
        private readonly CloudTable mapTable;
        private readonly CloudTable resourceTable;

        public ScampAzureContext()
        {
            var cloud = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            var prefix = CloudConfigurationManager.GetSetting("ProvisionTable");
            resourceTable = cloud.CreateCloudTableClient().GetTableReference(prefix + "Resources");
            mapTable = cloud.CreateCloudTableClient().GetTableReference(prefix + "Map");
            resourceTable.CreateIfNotExists();
            mapTable.CreateIfNotExists();
            client = new HttpClient();
        }

        public async Task<string> GetToken()
        {
            var items = new Dictionary<string, string>
            {
                {"grant_type", "client_credentials"},
                {"client_secret", CloudConfigurationManager.GetSetting("ApplicationSecret")},
                {"client_id", CloudConfigurationManager.GetSetting("ApplicationId")},
                {"resource", "https://management.core.windows.net/"}
            };

            string uri = String.Format("https://login.microsoftonline.com/{0}/oauth2/token", CloudConfigurationManager.GetSetting("TenantId"));

            var request = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = new FormUrlEncodedContent(items)
            };

            var jsonResult = await (await client.SendAsync(request)).Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<AccessToken>(jsonResult).Token;
        }

        public ICourse AddCourse(ICourse course)
        {
            var inserted = InsertItem(new CourseEntity(course));
            return inserted.ToCourse();
        }

        public IEnumerable<ICourse> Courses
        {
            get
            {
                var query = from c in resourceTable.CreateQuery<CourseEntity>()
                    where c.PartitionKey == CourseEntity.EntityKey
                    select c;

                return query.ToList().Select(c => c.ToCourse());
            }
        }

        public ICourse GetCourse(int id)
        {
            return (from c in Courses where c.Id == id select c).SingleOrDefault();
        }

        public ICourse UpdateCourse(ICourse c)
        {
            var entity = new CourseEntity(c);
            resourceTable.Execute(TableOperation.InsertOrReplace(entity));

            return entity.ToCourse();
        }

        public void AddStudentToCourse(IStudent student, ICourse course)
        {
            var existingStudent = GetStudent(student.MicrosoftId);

            if (existingStudent == null)
            {
                student = AddStudent(student);
            }

            mapTable.Execute(student.In(course));
        }

        public IStudent AddStudent(IStudent student)
        {
            var inserted = InsertItem(new StudentEntity(student));
            return inserted.ToStudent();
        }

        public IEnumerable<IStudent> GetStudents()
        {
            var query = from i in resourceTable.CreateQuery<StudentEntity>()
                where i.PartitionKey == StudentEntity.EntityKey
                select i.ToStudent();

            return query.ToList();
        }

        public IStudent GetStudent(string id)
        {
            return (from i in resourceTable.CreateQuery<StudentEntity>()
                where i.PartitionKey == StudentEntity.EntityKey &&
                      i.RowKey == id
                select i).SingleOrDefault().ToStudent();
        }

        public IEnumerable<IStudent> GetStudentsInCourse(ICourse course)
        {
            var query = from s in mapTable.CreateQuery<JoinEntity>()
                where s.PartitionKey == course.LinkKey()
                select s;


            return query.ToList().Select(s => GetStudent(s.RowKey));
        }

        public IEnumerable<IResource> GetResoucesForStudentInCourse(ICourse c, IStudent s)
        {
            var resources = from x in mapTable.CreateQuery<JoinEntity>()
                where x.PartitionKey == c.ResourceKey(s)
                select x.RowKey;

            foreach (var r in resources.ToList().Select(GetWebSite))
            {
                yield return new WebSite
                {
                    FtpPassword = r.FtpPassword,
                    FtpUsername = r.FtpUsername,
                    FtpUri = r.FtpSite == null ? null : new Uri(r.FtpSite),
                    WebSiteUri = r.WebSite == null ? null : new Uri(r.WebSite)
                };
            }
        }

        public void AddWebSite(ICourse course, IStudent student, IWebSite webSite)
        {
            var entity = new WebSiteEntity(webSite);
            var insert = TableOperation.Insert(entity);
            resourceTable.Execute(insert);

            var linkEntity = new JoinEntity(course.ResourceKey(student), entity.RowKey);
            mapTable.Execute(TableOperation.Insert(linkEntity));
        }

        private WebSiteEntity GetWebSite(string id)
        {
            return (from x in resourceTable.CreateQuery<WebSiteEntity>()
                where x.PartitionKey == WebSiteEntity.PartitionKeyName
                      && x.RowKey == id
                select x).FirstOrDefault();
        }

        private T InsertItem<T>(T item) where T : IEntityWithId
        {
            bool success = false;

            while (!success)
            {
                try
                {
                    resourceTable.Execute(TableOperation.Insert(item));
                    success = true;
                }
                catch
                {
                    item.IncrementId();
                }
            }

            return item;
        }
    }
}
