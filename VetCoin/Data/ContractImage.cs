using Discord.Commands.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VetCoin.Data
{
    public class ContractImage
    {
        public int Id { get; set; }


        public int ContractId { get; set; }

        public byte[] ImageContent { get; set; }

        public virtual Contract Contract{get;set;}
    }
}
