using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VetCoin.Data
{
    public class VetMember
    {
        public int Id { get; set; }

        public ulong DiscordId { get; set; }

        public string Name { get; set; }

        public string AvatarId { get; set; }

        public MemberType MemberType { get; set; }

        public virtual ICollection<CoinTransaction> RecivedTransactions { get; set; }
        public virtual ICollection<CoinTransaction> SendTransactions { get; set; }

    }


    public enum MemberType
    {
        User,   
        Issuer,
        Vault,
        Bank,
        Escrow
    }

}
