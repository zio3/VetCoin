using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VetCoin.Data;
using VetCoin.Data.JsonParamEntites;

namespace VetCoin.Codes
{
    public class SiteContext
    {
        public SiteContext(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            // DbContext = dbContext;
            //SiteTitle = configuration.GetValue<string>(nameof(SiteTitle));
            //CurrenryUnit = configuration.GetValue<string>(nameof(CurrenryUnit));
            //TopImageUrl = configuration.GetValue<string>(nameof(TopImageUrl));

            //DiscordAppClientId = configuration.GetValue<string>(nameof(DiscordAppClientId));

            //LoginCheckDiscordServerId = configuration.GetValue<ulong>(nameof(LoginCheckDiscordServerId));

            //SuperChatLowLimit = configuration.GetValue<int>(nameof(SuperChatLowLimit));

            //SuperChatHeightLimit = configuration.GetValue<int>(nameof(SuperChatHeightLimit));

            //AdminDiscordId = configuration.GetValue<ulong>(nameof(AdminDiscordId));
            //DeveloperDiscordId = configuration.GetValue<ulong>(nameof(DeveloperDiscordId));

            //SiteBaseUrl = configuration.GetValue<string>(nameof(SiteBaseUrl));

            //UseRegularDistribution = configuration.GetValue<bool>(nameof(UseRegularDistribution));
            // SiteSetting = DbContext.GetParam<SiteSetting>();
        }

        //[Obsolete]
        //public string SiteTitle { get; private set; }

        //[Obsolete]
        //public string CurrenryUnit { get; private set; }

        //[Obsolete]
        //public string TopImageUrl { get; private set; }

        //[Obsolete]
        //public string DiscordAppClientId { get; private set; }

        //[Obsolete]
        //public ulong LoginCheckDiscordServerId { get; private set; }

        //[Obsolete]
        //public int SuperChatLowLimit { get; private set; }

        //[Obsolete]
        //public int SuperChatHeightLimit { get; private set; }
        
        //[Obsolete]
        //public string SiteBaseUrl { get; private set; }

        //[Obsolete]
        //public ulong AdminDiscordId { get; private set; }

        //[Obsolete]
        //public ulong DeveloperDiscordId { get; private set; }

        //[Obsolete]
        //public bool UseRegularDistribution { get; set; }


        public SiteSetting _SiteSetting;
        public SiteSetting SiteSetting
        {
            get
            {
                if(_SiteSetting == null)
                {
                    using (var scope = this.ServiceProvider.CreateScope())
                    {
                        var context = ActivatorUtilities.CreateInstance<ApplicationDbContext>(scope.ServiceProvider);
                        _SiteSetting = context.GetParam<SiteSetting>();
                    }
                }
                return _SiteSetting;
            }
        }

        public void UpdateSiteSetting(SiteSetting siteSetting)
        {
            _SiteSetting = siteSetting;
        }

        ReactionMap[] _ReactionMaps;
        public ReactionMap[] ReactionMaps
        {
            get
            {
                if (_ReactionMaps == null)
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
        public ApplicationDbContext DbContext { get; }
    }
}




