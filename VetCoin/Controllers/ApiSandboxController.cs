using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetCoin.Codes;
using VetCoin.Data;
using VetCoin.Services;
using VetCoin.Services.Chat;
using VetCoin.Services.HostedServices;

namespace VetCoin.Controllers
{
#if DEBUG
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ApiSandboxController : ControllerBase
    {
        public ApiSandboxController(SuperChatService superChatService, DiscordService discordService, ApplicationDbContext applicationDbContext, SiteContext siteContext, CoreService coreService, IconCheckService iconCheckService)
        {
            SuperChatService = superChatService;
            DiscordService = discordService;
            DbContext = applicationDbContext;
            SiteContext = siteContext;
            CoreService = coreService;
            IconCheckService = iconCheckService;
        }

        public SuperChatService SuperChatService { get; }
        public DiscordService DiscordService { get; }
        public Data.ApplicationDbContext DbContext { get; }
        public SiteContext SiteContext { get; }
        public CoreService CoreService { get; }
        public IconCheckService IconCheckService { get; }

        [HttpGet]
        public async Task TestAsync()
        {
            var aa = CoreService.GetUserContext();

            var items = DbContext.Donations
                .AsQueryable()
                .Include(c => c.VetMember)
                //.Where(c=>c.DonationState == DonationState.Open)
                .ToArray();

            foreach (var item in items)
            {
                aa.CurrentUser = item.VetMember;
                await Notification(item, aa);
                await Task.Delay(1000);
            }
        }

        [HttpPost]
        public async Task IconCheck()
        {
            await IconCheckService.IconCheck();
        }


        private async Task Notification(Donation Donation, UserContext userContext)
        {
            await DiscordService.SendMessage(DiscordService.Channel.CrowdFundingNotification, string.Empty, new DiscordService.DiscordEmbed
            {
                title = Donation.Title,
                url = $"{SiteContext.SiteBaseUrl}Donations/Details?id={Donation.Id}",
                author = new DiscordService.DiscordEmbed.Author
                {
                    url = userContext.CurrentUser.GetMemberPageUrl(SiteContext.SiteBaseUrl),
                    icon_url = userContext.CurrentUser.GetAvaterIconUrl(),
                    name = userContext.CurrentUser.Name
                },
                fields = new DiscordService.DiscordEmbed.Field[]
             {
                    new DiscordService.DiscordEmbed.Field
                    {
                        name = "内容",
                        value = Donation.Content,
                        inline = false
                    }
             },
            });
        }

    }
#endif
}
