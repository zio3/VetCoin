using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VetCoin.Data.VenerEntityes
{
    public class Vender
    {
        public int Id { get; set; }
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        public int VetMemberId { get; set; }

        [DisplayName("Member")]
        public virtual VetMember VetMember { get; set; }

        public bool HasDeliveryMessage { get; set; }

        [DataType(DataType.MultilineText)]
        public string DeliveryMessage { get; set; }


        public string CreateUser { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTimeOffset UpdateDate { get; set; }

        public bool IsClosed { get; set; }

        public virtual ICollection<VenderSale> VenderSales { get; set; }

    }
}
