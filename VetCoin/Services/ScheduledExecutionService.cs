using VetCoin.Services.Chat;
using VetCoin.Services.HostedServices;
using System.Threading.Tasks;
using VetCoin.Codes;
using System;

namespace VetCoin.Services
{
    public class ScheduledExecutionService
    {

        public ScheduledExecutionService(
            DiscordService discordService,
            CoreService coreService,
            IconCheckService iconCheckService,
            StaticSettings staticSettings
            )
        {
            DiscordService = discordService;
            CoreService = coreService;
            IconCheckService = iconCheckService;
            StaticSettings = staticSettings;
        }

        public DiscordService DiscordService { get; }
        public CoreService CoreService { get; }
        public IconCheckService IconCheckService { get; }
        public StaticSettings StaticSettings { get; }
        public int Timespan { get; private set; }

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
            await Task.Delay(TimeSpan.FromSeconds(10));

            if (StaticSettings.UseRegularDistribution)
            {
                //await DiscordService.SendMessage(DiscordService.Channel.TEST, "VetCoin SendTest");
                await CoreService.RegularDistribution();
                await CoreService.DbContext.SaveChangesAsync();
            }
        }
    }



}
