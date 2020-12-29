using VetCoin.Services.Chat;
using VetCoin.Services.HostedServices;
using System.Threading.Tasks;
using VetCoin.Codes;

namespace VetCoin.Services
{
    public class ScheduledExecutionService
    {

        public ScheduledExecutionService(
            DiscordService discordService,
            CoreService coreService,
            IconCheckService iconCheckService,
            SiteContext siteContext
            )
        {
            DiscordService = discordService;
            CoreService = coreService;
            IconCheckService = iconCheckService;
            SiteContext = siteContext;
        }

        public DiscordService DiscordService { get; }
        public CoreService CoreService { get; }
        public IconCheckService IconCheckService { get; }
        public SiteContext SiteContext { get; }

        //[Cron("* * * * *")]
        //public async Task CollectEngage()
        //{
        //    await DiscordService.SendMessage(DiscordService.Channel.TEST, "VetCoin SendTest");
        //}
        [Cron("0 * * * *")]
        public async Task IconCheck()
        {
            await IconCheckService.IconCheck();
        }

        [Cron("0 0 1 * *")]
        public async Task RegularDistribution()
        {
            if (SiteContext.UseRegularDistribution)
            {
                //await DiscordService.SendMessage(DiscordService.Channel.TEST, "VetCoin SendTest");
                await CoreService.RegularDistribution();
                await CoreService.DbContext.SaveChangesAsync();
            }
        }
    }



}
