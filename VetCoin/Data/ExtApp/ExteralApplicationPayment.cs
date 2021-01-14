using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VetCoin.Data.ExtApp
{
    public class ExteralApplicationPayment : ICreate,IUpdate
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ExteralApplicationId { get; set; }

        public virtual ExteralApplication ExteralApplication { get; set; }

        public int Amount { get; set; }

        public string Description { get; set; }

        public ulong DiscordId { get; set; }

        public bool IsPayd { get; set; }

        public string RefJson { get; set; }

        //5分後?
        public DateTimeOffset ExpirationDate { get; set; }

        public string CreateUser { get; set; }
        public DateTimeOffset CreateDate { get; set; }

        public string UpdateUser { get; set; }
        public DateTimeOffset UpdateDate { get; set; }
    }
}
