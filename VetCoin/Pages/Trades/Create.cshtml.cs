using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using VetCoin.Data;
using VetCoin.Codes;
using VetCoin.Services;
using VetCoin.Services.Chat;

namespace VetCoin.Pages.Trades
{
    public class CreateModel : PageModel
    {
        private readonly VetCoin.Data.ApplicationDbContext DbContext;
        private readonly CoreService CoreService;

        [BindProperty(SupportsGet = true)]
        public Direction? Direction { get; set; }

        [BindProperty]
        [DisplayName("通知不要")]
        public bool IsSkipNotification { get; set; }

        public CreateModel(ApplicationDbContext context, CoreService coreService, DiscordService discordService, SiteContext siteContext)
        {
            DbContext = context;
            CoreService = coreService;
            DiscordService = discordService;
            SiteContext = siteContext;
        }

        public IActionResult OnGet(Direction? direction, int? cloneSrcId)
        {
            Direction = direction;

            if (Direction.HasValue)
            {
                Trade = new Trade { Direction = Direction.Value };
            }

            if (cloneSrcId.HasValue)
            {
                var src = DbContext.Trades.Find(cloneSrcId);
                Trade = src.Clone();
            }

            return Page();
        }

        [BindProperty]
        public Trade Trade { get; set; }
        public DiscordService DiscordService { get; }
        public SiteContext SiteContext { get; }

        private string GetDisplayName(Direction direction)
        {
            return direction.GetApplied<DisplayAttribute>().First().Name;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var userContext = CoreService.GetUserContext();
            Trade.VetMemberId = userContext.CurrentUser.Id;
            Trade.OrderRefDate = DateTimeOffset.Now;

            DbContext.Trades.Add(Trade);
            await DbContext.SaveChangesAsync();


            if (!IsSkipNotification)
            {
                await Notification(userContext);
            }

            return RedirectToPage("./Index", new { direction = Trade.Direction });
        }

        private async Task Notification(UserContext userContext)
        {
            await DiscordService.SendMessage(DiscordService.Channel.TradeEntryNotification, string.Empty, new DiscordService.DiscordEmbed
            {
                title = Trade.Title,
                url = $"https://vetcoin.azurewebsites.net/Trades/Details?id={Trade.Id}",
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
                        name = "売買",
                        value = $"{GetDisplayName(Trade.Direction)}",
                        inline = false
                    },
                    new DiscordService.DiscordEmbed.Field
                    {
                        name = "希望価格",
                        value = GetPriceStr(Trade),
                        inline = true
                    },
                    new DiscordService.DiscordEmbed.Field
                    {
                        name = "納期",
                        value = Trade.DeliveryDate??"[未設定]",
                        inline = true
                    },
                    new DiscordService.DiscordEmbed.Field
                    {
                        name = "依頼内容",
                        value = Trade.Content,
                        inline = false
                    }
            },
            });
        }

        string GetPriceStr(Trade trade)
        {
            var str = $"{Trade.Reward} {Trade.RewardComment ?? string.Empty}";
            if(string.IsNullOrEmpty(str.Trim()))
            {
                str = "[未設定]";
            }
            return str;
        }
    }
}