using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SCAMP.Provisions;

namespace SCAMP.Provisioning
{
    [DataContract]
    public class ProvisionRequest
    {
        [DataMember]
        public string AccountId { get; set; }
        [DataMember]
        public Guid SubscriptionId { get; set; }
        [DataMember]
        public string ServicePlanName { get; set; }
        [DataMember]
        public string GroupPrefix { get; set; }
        [DataMember]
        public string AuthorizationToken { get; set; }
        [DataMember]
        public Person Professor { get; set; }
        [DataMember]
        public IEnumerable<Person> Students { get; set; }
    }
}