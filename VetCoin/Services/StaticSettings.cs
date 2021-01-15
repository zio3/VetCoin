using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VetCoin.Services
{
    public class StaticSettings
    {


        public string SiteTitle { get; init; }

        public string CurrenryUnit { get; init; }

        public string TopImageUrl { get; init; }

        public string DiscordAppClientId { get; init; }
        public ulong LoginCheckDiscordServerId { get; init; }

        public int SuperChatLowLimit { get; init; }
        public int SuperChatHeightLimit { get; init; }
        public string SiteBaseUrl { get; init; }

        public ulong AdminDiscordId { get; init; }

        public ulong DeveloperDiscordId { get; init; }

        public bool UseRegularDistribution { get; init; }

        public bool EnableHostedService { get; set; }
    }
}
