using System;
using System.Runtime.Serialization;

namespace SCAMP.Provisions
{
    [DataContract]
    public class Person
    {
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string MicrosoftId { get; set; }
        [DataMember]
        public Guid UserId { get; set; }
    }
}