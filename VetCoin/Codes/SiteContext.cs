using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VetCoin.Data;

namespace VetCoin.Codes
{
    public class SiteContext
    {
        public SiteContext(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;

            SiteTitle = configuration.GetValue<string>(nameof(SiteTitle));
            CurrenryUnit = configuration.GetValue<string>(nameof(CurrenryUnit));
            TopImageUrl = configuration.GetValue<string>(nameof(TopImageUrl));

            DiscordAppClientId = configuration.GetValue<string>(nameof(DiscordAppClientId));

            LoginCheckDiscordServerId = configuration.GetValue<ulong>(nameof(LoginCheckDiscordServerId));

            SuperChatLowLimit = configuration.GetValue<int>(nameof(SuperChatLowLimit));

            SuperChatHeightLimit = configuration.GetValue<int>(nameof(SuperChatHeightLimit));

            AdminDiscordId = configuration.GetValue<ulong>(nameof(AdminDiscordId));
            DeveloperDiscordId = configuration.GetValue<ulong>(nameof(DeveloperDiscordId));
            
        }

        public string SiteTitle { get; private set; }

        public string CurrenryUnit { get; private set; }

        public string TopImageUrl { get; private set; }

        public string DiscordAppClientId { get; private set; }
        public ulong LoginCheckDiscordServerId { get; private set; }

        public int SuperChatLowLimit { get; private set; }
        public int SuperChatHeightLimit { get; private set; }

        public ulong AdminDiscordId { get; private set; }

        public ulong DeveloperDiscordId { get; private set; }


        ReactionMap[] _ReactionMaps;
        public ReactionMap[] ReactionMaps 
        {
            get
            {
                if(_ReactionMaps == null)
                {
                    using (var scope = ServiceProvider.CreateScope())
                    {
                        var dbContext = ActivatorUtilities.CreateInstance<ApplicationDbContext>(scope.ServiceProvider);
                        _ReactionMaps = dbContext.ReactionMaps.ToArray();
                    }
                }
                return _ReactionMaps;
            }
        }

        public void ClearReactionMap()
        {
            _ReactionMaps = null;
        }

        public IServiceProvider ServiceProvider { get; }
    }
}




