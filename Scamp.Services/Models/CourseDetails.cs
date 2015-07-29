using System.Collections.Generic;
using System.Runtime.Serialization;
using SCAMP.Contracts;

namespace SCAMP.Models
{
    [DataContract]
    [KnownType(typeof (StudentWithSite))]
    public class CourseDetails : Course
    {
        public CourseDetails(ICourse course)
            : base(course)
        {
        }

        [DataMember]
        public IEnumerable<IStudent> Students { get; set; }
    }
}
