using System;
using Microsoft.WindowsAzure.Storage.Table;
using SCAMP.Contracts;

namespace SCAMP.Azure
{
    public class CourseEntity : TableEntity, IEntityWithId
    {
        internal const string EntityKey = "Course";

        public CourseEntity()
            : base(EntityKey, "0")
        {
        }

        public CourseEntity(ICourse course)
            : this()
        {
            RowKey = course.Id.ToString();
            Name = course.Name;
            ExpirationTime = course.ExpirationTime;
            ProfessorId = course.Professor.Id;
            State = course.State.ToString();
        }

        public string Name { get; set; }
        public DateTimeOffset ExpirationTime { get; set; }
        public int ProfessorId { get; set; }

        public int Id
        {
            get { return int.Parse(RowKey); }
            private set { RowKey = value.ToString(); }
        }

        public string State { get; set; }

        public void IncrementId()
        {
            Id ++;
        }
    }
}
