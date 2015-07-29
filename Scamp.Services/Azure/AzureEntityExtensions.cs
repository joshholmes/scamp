using System;
using Microsoft.WindowsAzure.Storage.Table;
using SCAMP.Contracts;
using SCAMP.Models;

namespace SCAMP.Azure
{
    public static class AzureEntityExtensions
    {
        public static ICourse ToCourse(this CourseEntity entity)
        {
            return new Course
            {
                Id = int.Parse(entity.RowKey),
                ExpirationTime = entity.ExpirationTime,
                Name = entity.Name,
                State = (CourseState) Enum.Parse(typeof (CourseState), entity.State)
            };
        }

        public static IStudent ToStudent(this StudentEntity entity)
        {
            IStudent s = null;

            if (entity != null)
            {
                s = new Student
                {
                    FirstName = entity.FirstName,
                    LastName = entity.LastName,
                    MicrosoftId = entity.RowKey
                };
            }
            return s;
        }

        public static TableOperation In(this IStudent student, ICourse course)
        {
            return TableOperation.InsertOrMerge(new JoinEntity(course.LinkKey(), student.MicrosoftId));
        }

        public static string LinkKey(this ICourse course)
        {
            return String.Join(":", "Course", course.Id);
        }

        public static string ResourceKey(this ICourse course, IStudent student)
        {
            return String.Join(":", "Resource", course.Id, student.MicrosoftId);
        }
    }
}
