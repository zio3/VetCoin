using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VetCoin.Data
{
    public class SubscriptionMember : ICreate
    {
        public int SubscriptionId { get; set; }

        public int VetMemberId { get; set; }

        public Subscription Subscription { get; set; }

        public VetMember VetMember { get; set; }
        public string CreateUser { get; set; }
        public DateTimeOffset CreateDate { get; set; }
    }
}


