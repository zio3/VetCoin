using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VetCoin.Data
{
    public class TradeLikeVote : ICreate
    {
        public int Id { get; set; }

        public int TradeId { get; set; }

        public int VetMemberId { get; set; }
        public string CreateUser {get; set; }

        public DateTimeOffset CreateDate  {get; set; }

        public virtual VetMember VetMember { get; set; }

        public virtual Trade Trade { get; set; }
    }
}
