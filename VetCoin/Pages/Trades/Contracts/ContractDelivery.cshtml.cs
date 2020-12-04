using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VetCoin.Codes;
using VetCoin.Data;
using VetCoin.Services;

namespace VetCoin.Pages.Trades.Contracts
{
    public class ContractDeliveryModel : PageModel
    {
        private readonly VetCoin.Data.ApplicationDbContext DbContext;

        public ContractDeliveryModel(VetCoin.Data.ApplicationDbContext context, CoreService coreService,SiteContext siteContext)
        {
            DbContext = context;
            CoreService = coreService;
            SiteContext = siteContext;
        }

        [BindProperty]
        public Contract Contract { get; set; }
        public Trade Trade { get; set; }

        public UserContext UserContext { get; set; }
        public CoreService CoreService { get; }
        public SiteContext SiteContext { get; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            UserContext = CoreService.GetUserContext();

            Contract = await DbContext.Contracts
                .Include(c => c.Trade.VetMember)
                .Include(c => c.VetMember).FirstOrDefaultAsync(m => m.Id == id);

            Trade = Contract.Trade;

            var sellUser = Trade.Direction == Direction.Sell ?
                          DbContext.VetMembers.Find(Trade.VetMemberId) :
                          DbContext.VetMembers.Find(Contract.VetMemberId);

            if (UserContext.CurrentUser.Id != sellUser.Id)
            {
                return NotFound();
            }

            if (Contract == null)
            {
                return NotFound();
            }

            if (Contract.ContractStatus != ContractStatus.Working)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            //DbContext.Attach(Contract).State = EntityState.Modified;
            var entity = DbContext.Contracts
                .Include(c => c.EscrowTransaction)
                .First(c => c.Id == Contract.Id);
            var trade = DbContext.Trades.Find(entity.TradeId);
            //await TryUpdateModelAsync(entity, nameof(Contract));
            entity.ContractStatus = ContractStatus.Deliveryed;

            var escrowReciveUser = trade.Direction == Direction.Buy ?
                          DbContext.VetMembers.Find(trade.VetMemberId) :
                          DbContext.VetMembers.Find(entity.VetMemberId);
            var escrowSendUser = trade.Direction == Direction.Buy ?
                                      DbContext.VetMembers.Find(trade.VetMemberId) :
                                      DbContext.VetMembers.Find(entity.VetMemberId);


            if (trade.IsContinued)
            {
                trade.OrderRefDate = DateTimeOffset.Now;
            }

            try
            {
                await DbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContractExists(Contract.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            await SendMessages(entity, escrowSendUser, escrowReciveUser);

            return RedirectToPage("./Index", new { contractId = Contract.Id });
        }

        private bool ContractExists(int id)
        {
            return DbContext.Contracts.Any(e => e.Id == id);
        }


        //private async Task<VetMember[]> GetStakeHolders(int id)
        //{
        //    var contractMember = DbContext.Contracts.AsQueryable().Where(c => c.Id == id).Select(c => c.VetMember);
        //    var tradeMember = DbContext.Contracts.AsQueryable().Where(c => c.Id == id).Select(c => c.Trade.VetMember);
        //    var messageMembers = DbContext.TradeMessages.AsQueryable().Where(c => c.TradeId == id).Select(c => c.VetMember);

        //    var stakeHolders = contractMember.Concat(tradeMember).Concat(messageMembers).Distinct();


        //    return await stakeHolders.ToArrayAsync();
        //}

        private async Task SendMessages(Contract contract, VetMember sender, VetMember reciver)
        {
            //var stakeHolders = await GetStakeHolders(contract.Id);
            var trade = DbContext.Contracts.AsQueryable().Where(c => c.Id == contract.Id).Select(c => c.Trade).First();

            Discord.EmbedBuilder builder = new Discord.EmbedBuilder();
            builder.WithTitle(trade.Title)
            .WithAuthor(sender.Name, sender.GetAvaterIconUrl(), sender.GetMemberPageUrl(SiteContext.SiteBaseUrl))
            .WithUrl($"https://vetcoin.azurewebsites.net/Trades/Contracts?contractId={contract.Id}")
                .AddField("アクション", "作業完了通知")
                .AddField("メッセージ内容", @"作業完了したようです。
納品物などを確認して契約完了ボタンをおしてください");



            var messageTargets = new[] { sender };

            await CoreService.SendDirectMessage(messageTargets, string.Empty, builder.Build());
        }
    }
}
