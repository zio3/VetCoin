using VetCoin.Services.Chat;
using VetCoin.Services.HostedServices;
using System.Threading.Tasks;

namespace VetCoin.Services
{
    public class ScheduledExecutionService
    {

        public ScheduledExecutionService(
            DiscordService discordService,
            CoreService coreService,
            IconCheckService iconCheckService
            )
        {
            DiscordService = discordService;
            CoreService = coreService;
            IconCheckService = iconCheckService;
        }

        public DiscordService DiscordService { get; }
        public CoreService CoreService { get; }
        public IconCheckService IconCheckService { get; }

        //[Cron("* * * * *")]
        //public async Task CollectEngage()
        //{
        //    await DiscordService.SendMessage(DiscordService.Channel.TEST, "VetCoin SendTest");
        //}
        [Cron("0 * * * *")]
        public async Task RegularDistribution()
        {
            await IconCheckService.IconCheck();
        }

        [Cron("0 0 1 * *")]
        public async Task RegularDistribution()
        {
            //await DiscordService.SendMessage(DiscordService.Channel.TEST, "VetCoin SendTest");
            await CoreService.RegularDistribution();
            await CoreService.DbContext.SaveChangesAsync();
        }
    }



}
