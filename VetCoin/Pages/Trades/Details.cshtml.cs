using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VetCoin.Data;
using VetCoin.Services;

namespace VetCoin.Pages.Trades
{
    public class DetailsModel : PageModel
    {
        private readonly VetCoin.Data.ApplicationDbContext DbContext;

        public bool IsVoted { get; set; }

        public int VoteCount { get; set; }

        public DetailsModel(VetCoin.Data.ApplicationDbContext context, CoreService coreService)
        {
            DbContext = context;
            CoreService = coreService;
        }

        public Trade Trade { get; set; }

        public bool IsOwner { get; set; }
        public CoreService CoreService { get; }

        public string PostMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Trade = await DbContext.Trades
                .Include(t => t.VetMember)
                .Include(t => t.TradeMessages)                
                    .ThenInclude(c => c.VetMember)
                .Include(t => t.Contracts)
                    .ThenInclude(c=>c.VetMember)
                .FirstOrDefaultAsync(m => m.Id == id);

            var userContext = CoreService.GetUserContext();
            IsOwner = Trade.VetMemberId == userContext.CurrentUser.Id;


            if (Trade == null)
            {
                return NotFound();
            }

            VoteCount = await DbContext.TradeLikeVotes
                .AsQueryable()
                .CountAsync(c => c.TradeId == id);
            IsVoted = await DbContext.TradeLikeVotes
                .AsQueryable()
                .AnyAsync(c => c.TradeId == id && c.VetMemberId == userContext.CurrentUser.Id);

            return Page();
        }
        public async Task<IActionResult> OnPostAsync(int? id,string postMessage)
        {
            var trade = await DbContext.Trades.FindAsync(id);
            if(trade == null)
            {
                return NotFound();
            }

            if(!string.IsNullOrEmpty(postMessage))
            {
                var userContext = CoreService.GetUserContext();
                var tm = new TradeMessage
                {
                    Message = postMessage,
                    TradeId = trade.Id,
                    VetMemberId = userContext.CurrentUser.Id
                };

                DbContext.TradeMessages.Add(tm);
                DbContext.SaveChanges();


                await SendMessages(trade, userContext.CurrentUser, postMessage);


                PostMessage = null;
            }
            return await OnGetAsync(id);
        }

        private async Task SendMessages(Trade trade, VetMember sender,string message)
        {
            var senderIsOwner = trade.VetMemberId == sender.Id;

            var messageTargets =
                senderIsOwner ? DbContext.TradeMessages.AsQueryable().Where(c => c.TradeId == trade.Id).Select(c => c.VetMember).ToArray()
                            : new[] { Trade.VetMember };

            var dmMessage = $@"
メッセージ元:{trade.Title}
URL:https://vetcoin.azurewebsites.net/Trades/Details?id={trade.Id}
差出人:{sender.Name}
{message}";

            Discord.EmbedBuilder builder = new Discord.EmbedBuilder();
            builder.WithTitle(trade.Title)
                .WithAuthor(sender.Name, sender.GetAvaterIconUrl(), sender.GetMemberPageUrl())
                .WithUrl($"https://vetcoin.azurewebsites.net/Trades/Details?id={trade.Id}")
               // .AddField("アクション", "メッセージがあります")
                .AddField("メッセージ内容", message);





            await CoreService.SendDirectMessage(messageTargets, string.Empty, builder.Build());

        }
    }
}
