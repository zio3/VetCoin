using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VetCoin.Data.ExtApp
{
    public class ExteralApplication 
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; }

        public string CallbackUrl { get; set; }
        
        public int VetMemberId { get; set; }
        public virtual VetMember VetMember { get; set; }

        public bool IsNotification { get; set; }
        
        [JsonIgnore]
        public virtual ICollection<ExteralApplicationPayment> ExteralApplicationPayments { get; set; }
    }
}
