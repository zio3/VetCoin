using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace VetCoin.Data.VenerEntityes
{
    public class VenderSale
    {
        public int Id { get; set; }

        public int VenderId { get; set; }

        public virtual Vender Vender { get; set; }

        public int Amount { get; set; }

        public string Message { get; set; }

        public int VetMemberId { get; set; }

        [DisplayName("Member")]
        public virtual VetMember VetMember { get; set; }


        public string CreateUser { get; set; }
        public DateTimeOffset CreateDate { get; set; }

    }
}
