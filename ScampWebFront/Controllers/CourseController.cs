using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Newtonsoft.Json;
using SCAMP.Contracts;
using SCAMP.Models;
using ScampWebFront.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using SCAMP.Azure;

namespace ScampWebFront.Controllers
{
    public class CourseController : ApiController
    {
        private IScampContext db = new ScampAzureContext();

        // GET api/Course
        public IEnumerable<ICourse> GetCourses()
        {
            return db.Courses;
        }

        // GET api/Course/5
        [ResponseType(typeof(CourseDetails))]
        [HttpGet]
        public IHttpActionResult Details(int id)
        {
            ICourse course = db.GetCourse(id);

            if (course == null)
            {
                return NotFound();
            }

            var items = new List<StudentWithSite>();

            foreach (var student in db.GetStudentsInCourse(course))
            {
                items.Add(new StudentWithSite(student, db.GetResoucesForStudentInCourse(course, student).Select(x => (IWebSite)x).FirstOrDefault()));
            }

            var details = new CourseDetails(course)
            {
                Students = items,
            };

            return Ok(details);
        }

        [ResponseType(typeof(ICourse))]
        public IHttpActionResult GetCourse(int id)
        {
            ICourse course = db.GetCourse(id);

            if (course == null)
            {
                return NotFound();
            }

            return Ok(course);
        }

        // PUT api/Course/5
        [ResponseType(typeof (Course))]
        public IHttpActionResult PutCourse(int id, EditCourseViewModel course)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != course.Id)
            {
                return BadRequest();
            }

            ICourse c = db.UpdateCourse(course);

            foreach (IStudent student in course.Students)
            {
                db.AddStudentToCourse(student, c);
            }

            return GetCourse(id);
        }

        // POST api/Course
        [ResponseType(typeof(ICourse))]
        public IHttpActionResult PostCourse(Course course)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var addedCourse = db.AddCourse(course);

            return CreatedAtRoute("DefaultApi", new { id = addedCourse.Id }, addedCourse);
        }

        [ResponseType(typeof(CourseResources))]
        public IHttpActionResult GetCourseResources(int id)
        {
            ICourse course = db.GetCourse(id);

            if (course == null)
            {
                return NotFound();
            }

            var resources = db.GetStudentsInCourse(course).SelectMany(s => db.GetResoucesForStudentInCourse(course, s));
            return Ok(new CourseResources(course, resources));
        }

        // POST api/Course/{name}/resources/provision
        public async Task<IHttpActionResult> Provision(int id)
        {
            ICourse course = null;

            if (!ModelState.IsValid || (course = db.GetCourse(id)) == null)
            {
                return BadRequest(ModelState);
            }

            var request = new ProvisionRequest()
            {
                AccountId = Guid.Parse(CloudConfigurationManager.GetSetting("ApplicationId")),
                SubscriptionId = Guid.Parse(CloudConfigurationManager.GetSetting("SubscriptionId")),
                PrincipalOwnerId = Guid.Parse(CloudConfigurationManager.GetSetting("PrincipalOwnerId")),
                AuthorizationToken = await db.GetToken(),
                Course = course
            };

            TopicClient client = TopicClient.CreateFromConnectionString(CloudConfigurationManager.GetSetting("SBConnectionString"), CloudConfigurationManager.GetSetting("SBTopic"));
            client.Send(new BrokeredMessage(request));

            return Ok();
        }

        [HttpPut]
        public IHttpActionResult AddStudent(int id, Student newStudent)
        {
            ICourse course = db.GetCourse(id);

            if (course == null)
            {
                return NotFound();
            }

            IStudent student = db.GetStudent(newStudent.MicrosoftId);

            if (student == null)
            {
                student = db.AddStudent(newStudent);
            }

            db.AddStudentToCourse(student, course);

            return Ok();
        }

        private bool CourseExists(int id)
        {
            return db.GetCourse(id) != null;
        }
    }
}