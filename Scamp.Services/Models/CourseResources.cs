using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using SCAMP.Contracts;

namespace SCAMP.Models
{
    [DataContract]
    public class CourseResources : Course
    {
        public CourseResources()
        {
        }

        public CourseResources(ICourse course, IEnumerable<IResource> resources)
            : base(course)
        {
            Resources = resources.ToList().AsReadOnly();
        }

        [DataMember]
        public IEnumerable<IResource> Resources { get; set; }
    }
}
