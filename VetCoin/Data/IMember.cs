using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VetCoin.Data
{
    interface IMember
    {
        int VetMemberId { get; set; }
        VetMember VetMember { get; set; }
    }
}
