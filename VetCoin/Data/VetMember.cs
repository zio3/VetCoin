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
        public string Markdown { get; set; }

        public string GetAvaterIconUrl()
        {
            if(AvatarId != null)
            {
                return "https://discord.com/assets/322c936a8c8be1b803cd94861bdfa868.png";
            }

            return $"https://cdn.discordapp.com/avatars/{DiscordId}/{AvatarId}.png?size=128";
        }
        public string GetMemberPageUrl(string siteBaseUrl)
        {
            return $"{siteBaseUrl}Member/{DiscordId}";
        }
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
