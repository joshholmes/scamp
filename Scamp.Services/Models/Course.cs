using System;
using System.Runtime.Serialization;
using SCAMP.Azure;
using SCAMP.Contracts;

namespace SCAMP.Models
{
    [DataContract]
    [KnownType(typeof (Professor))]
    public class Course : ICourse
    {
        public Course()
        {
            ExpirationTime = DateTimeOffset.Now;
            Professor = new Professor();
        }

        public Course(ICourse course)
        {
            Id = course.Id;
            Name = course.Name;
            ExpirationTime = course.ExpirationTime;
            Professor = course.Professor;
            State = course.State;
        }

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public DateTimeOffset ExpirationTime { get; set; }

        [DataMember]
        public IProfessor Professor { get; set; }

        [DataMember]
        public CourseState State { get; set; }
    }
}
