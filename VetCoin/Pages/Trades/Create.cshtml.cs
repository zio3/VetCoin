using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using VetCoin.Data;
using VetCoin.Services;
using VetCoin.Services.Chat;

namespace VetCoin.Pages.Trades
{
    public class CreateModel : PageModel
    {
        private readonly VetCoin.Data.ApplicationDbContext DbContext;
        private readonly CoreService CoreService;

        [BindProperty(SupportsGet =true)]
        public Direction? Direction { get; set; }

        [BindProperty]
        [DisplayName("通知不要")]
        public bool IsSkipNotification { get; set; }

        public CreateModel(ApplicationDbContext context, CoreService coreService,DiscordService discordService)
        {
            DbContext = context;
            CoreService = coreService;
            DiscordService = discordService;
        }

        public IActionResult OnGet(Direction? direction,int? cloneSrcId)
        {
            Direction = direction;

            if(Direction.HasValue)
            {
                Trade = new Trade { Direction = Direction.Value };
            }

            if(cloneSrcId.HasValue)
            {
                var src = DbContext.Trades.Find(cloneSrcId);
                Trade = src.Clone();
            }            

            return Page();
        }

        [BindProperty]
        public Trade Trade { get; set; }
        public DiscordService DiscordService { get; }

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

                await DiscordService.SendMessage(DiscordService.Channel.TradeEntryNotification, $@"{Trade.Direction}
タイトル:{Trade.Title}
依頼主:{userContext.CurrentUser.Name}
料金:{Trade.Reward} {Trade.RewardComment}
納期:{Trade.DeliveryDate}

{Trade.Content}

https://vetcoin.azurewebsites.net/Trades/Details?id={Trade.Id}");
            }

            return RedirectToPage("./Index",new {direction=Trade.Direction });
        }
    }
}