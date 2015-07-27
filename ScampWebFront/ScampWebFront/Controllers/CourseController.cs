using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using ScampWebFront.Models;

namespace ScampWebFront.Controllers
{
    public class CourseController : ApiController
    {
        private ScampWebFrontContext db = new ScampWebFrontContext();

        // GET api/Course
        public IQueryable<Course> GetCourses()
        {
            var courses = db.Courses;
            return courses;
        }

        // GET api/Course/5
        [ResponseType(typeof(Course))]
        public IHttpActionResult GetCourse(int id)
        {
            Course course = db.Courses.Include(p => p.Students).Single(c=>c.Id == id);
            if (course == null)
            {
                return NotFound();
            }



            return Ok(course);
        }

        // PUT api/Course/5
        public IHttpActionResult PutCourse(int id, Course course)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != course.Id)
            {
                return BadRequest();
            }

            db.Entry(course).State = EntityState.Modified;
            foreach (var student in course.Students)
            {
                if (student.Id != 0)
                {
                    db.Entry(student).State = EntityState.Modified;
                }
                else
                {
                    db.Entry(student).State = EntityState.Added;
                }
            }
            db.Entry(course).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST api/Course
        [ResponseType(typeof(Course))]
        public IHttpActionResult PostCourse(Course course)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Courses.Add(course);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = course.Id }, course);
        }

        // POST api/Course/{name}/resources/provision
        [ResponseType(typeof(Course))]
        public IHttpActionResult ResourceProvision(int id, Course course)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            foreach (Student s in course.Students) 
            {
                
            }

            return Ok(course);
        }

        // DELETE api/Course/5
        [ResponseType(typeof(Course))]
        public IHttpActionResult DeleteCourse(int id)
        {
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return NotFound();
            }

            db.Courses.Remove(course);
            db.SaveChanges();

            return Ok(course);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CourseExists(int id)
        {
            return db.Courses.Count(e => e.Id == id) > 0;
        }
    }
}