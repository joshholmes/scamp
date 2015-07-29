using System;
using System.Runtime.Serialization;
using SCAMP.Models;

namespace SCAMP.Contracts
{
    [DataContract]
    [KnownType(typeof (Course))]
    public class ProvisionRequest
    {
        [DataMember]
        public Guid AccountId { get; set; }

        [DataMember]
        public Guid SubscriptionId { get; set; }

        [DataMember]
        public Guid PrincipalOwnerId { get; set; }

        [DataMember]
        public string AuthorizationToken { get; set; }

        [DataMember]
        public ICourse Course { get; set; }
    }
}
