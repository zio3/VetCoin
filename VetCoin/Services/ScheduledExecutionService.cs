using VetCoin.Services.Chat;
using VetCoin.Services.HostedServices;
using System.Threading.Tasks;

namespace VetCoin.Services
{
    public class ScheduledExecutionService
    {

        public ScheduledExecutionService(
            DiscordService discordService,
            CoreService coreService
            )
        {
            DiscordService = discordService;
            CoreService = coreService;
        }

        public DiscordService DiscordService { get; }
        public CoreService CoreService { get; }

        //[Cron("* * * * *")]
        //public async Task CollectEngage()
        //{
        //    await DiscordService.SendMessage(DiscordService.Channel.TEST, "VetCoin SendTest");
        //}


        //[Cron("0 0 * * *")]
        //public async Task CollectEngage()
        //{
        //    //await DiscordService.SendMessage(DiscordService.Channel.TEST, "VetCoin SendTest");
        //    await CoreService.RegularDistribution();
        //    await CoreService.DbContext.SaveChangesAsync();
        //}
    }



}
