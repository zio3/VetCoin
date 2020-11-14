using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VetCoin.Data;
using VetCoin.Services;

namespace VetCoin.Pages.Trades.Contracts
{
    public class IndexModel : PageModel
    {
        private readonly VetCoin.Data.ApplicationDbContext DbContext;

        public IndexModel(VetCoin.Data.ApplicationDbContext context, CoreService coreService)
        {
            DbContext = context;
            CoreService = coreService;
        }

        public Contract Contract { get; set; }

        public string PostMessage { get; set; }
        public CoreService CoreService { get; }

        public UserContext UserContext { get; set; }

        public async Task<IActionResult> OnGetAsync(int? contractId)
        {
            if (contractId == null)
            {
                return NotFound();
            }

            UserContext = CoreService.GetUserContext();

            Contract = await DbContext.Contracts
                .Include(c => c.VetMember)
                .Include(c => c.Trade)
                    .ThenInclude(c => c.VetMember)
                .Include(c => c.ContractMessages)
                    .ThenInclude(c => c.VetMember)
                .FirstOrDefaultAsync(m => m.Id == contractId);

            if (Contract == null)
            {
                return NotFound();
            }
            return Page();
        }


        public async Task<IActionResult> OnPostAsync(int? contractId, string postMessage)
        {
            var contract = await DbContext.Contracts.FindAsync(contractId);
            if (contract == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(postMessage))
            {
                var userContext = CoreService.GetUserContext();
                var cm = new ContractMessage
                {
                    Message = postMessage,
                    ContractId = contract.Id,
                    VetMemberId = userContext.CurrentUser.Id
                };

                DbContext.ContractMessage.Add(cm);
                DbContext.SaveChanges();
                PostMessage = null;

                await SendMessages(contract, userContext.CurrentUser, postMessage);

            }
            return await OnGetAsync(contractId);
        }

        private async Task<VetMember[]> GetStakeHolders(int id)
        {
            var contractMember = DbContext.Contracts.AsQueryable().Where(c => c.Id == id).Select(c => c.VetMember);
            var tradeMember = DbContext.Contracts.AsQueryable().Where(c => c.Id == id).Select(c => c.Trade.VetMember);
            
            var stakeHolders = contractMember.Concat(tradeMember).Distinct();
            return await stakeHolders.ToArrayAsync();
        }

        private async Task SendMessages(Contract contract, VetMember sender, string message)
        {
            var stakeHolders = await GetStakeHolders(contract.Id);
            var trade = DbContext.Contracts.AsQueryable().Where(c => c.Id == contract.Id).Select(c => c.Trade).First();


            //            var dmMessage = $@"
            //メッセージ元:{trade.Title}
            //URL:https://vetcoin.azurewebsites.net/Trades/Contracts?contractId={contract.Id}
            //差出人:{sender.Name}
            //{message}";
            Discord.EmbedBuilder builder = new Discord.EmbedBuilder();
            builder.WithTitle(trade.Title)
                .WithAuthor(sender.Name, sender.GetAvaterIconUrl(), sender.GetMemberPageUrl())
                .WithUrl($"https://vetcoin.azurewebsites.net/Trades/Contracts?contractId={contract.Id}")
                // .AddField("アクション", "メッセージがあります")
                .AddField("メッセージ内容", message);


            var messageTargets = stakeHolders.Where(c => c.Id != sender.Id).ToArray();


//#if DEBUG
//            messageTargets = new[] { sender };
//#endif

            await CoreService.SendDirectMessage(messageTargets, string.Empty, builder.Build());
        }
    }
}
