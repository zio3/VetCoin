using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VetCoin.Data
{
    public class TradeImage
    {
        public int Id { get; set; }

        public int TradeId { get; set; }

        public byte[] ImageContent { get; set; }

        public virtual Trade Trade { get; set; }
    }
}
