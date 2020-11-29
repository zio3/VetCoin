using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VetCoin.Data
{
    public class DonationLog : ICreate
    {
        public int Id { get; set; }
        public int DonationId { get; set; }
        public virtual Donation Donation { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }
        public string CreateUser { get; set; }
        public DateTimeOffset CreateDate { get; set; }
    }
}
