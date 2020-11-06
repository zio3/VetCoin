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


        private async Task<VetMember[]> GetStakeHolders(int id)
        {
            var tradeMember = DbContext.Trades.AsQueryable().Where(c => c.Id == id).Select(c => c.VetMember);
            var messageMembers = DbContext.TradeMessages.AsQueryable().Where(c => c.TradeId == id).Select(c => c.VetMember);

            var stakeHolders = tradeMember.Concat(messageMembers).Distinct();
            return await stakeHolders.ToArrayAsync();
        }

        private async Task SendMessages(Trade trade, VetMember sender,string message)
        {
            var stakeHolders = await GetStakeHolders(trade.Id);


            var dmMessage = $@"
メッセージ元:{trade.Title}
URL:https://vetcoin.azurewebsites.net/Trades/Details?id={trade.Id}
差出人:{sender.Name}
{message}";

            var messageTargets = stakeHolders.Where(c => c.Id != sender.Id).ToArray();

            await CoreService.SendDirectMessage(messageTargets, dmMessage);

        }
    }
}
