using SCAMP.Azure;
using SCAMP.Contracts;
using System;
using System.Collections.Generic;

namespace ScampWebFront.Models
{
    public class EditCourseViewModel : ICourse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<NewStudentViewModel> Students { get; set; }
        public DateTimeOffset ExpirationTime { get; set; }
        public IProfessor Professor { get; set; }
        public CourseState State { get; set; }
        public Decimal Cost { get; set; }
    }

    public class NewStudentViewModel : IStudent
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MicrosoftId { get; set; }
        public int Id { get; set; }
    }
}