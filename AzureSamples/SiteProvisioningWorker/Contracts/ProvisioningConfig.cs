using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCAMP.Contracts
{
    class ProvisioningConfig
    {
        public Guid AccountId { get; set; }
        public Guid SubscriptionId { get; set; }
        public String AuthorizationToken { get; set; }
    }
}
