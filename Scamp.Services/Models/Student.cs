using System.Runtime.Serialization;
using SCAMP.Contracts;

namespace SCAMP.Models
{
    [DataContract]
    public class Student : IStudent
    {
        public Student()
        {
        }

        public Student(IStudent student)
        {
            FirstName = student.FirstName;
            LastName = student.LastName;
            MicrosoftId = student.MicrosoftId;
        }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string MicrosoftId { get; set; }
    }
}
