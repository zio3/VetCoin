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
    public class ContractAgreementModel : PageModel
    {

        private readonly VetCoin.Data.ApplicationDbContext DbContext;

        public ContractAgreementModel(VetCoin.Data.ApplicationDbContext context, CoreService coreService,SiteContext siteContext)
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
        public bool IsInsufficientFunds { get; set; }

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


            if (Contract == null)
            {
                return NotFound();
            }

            if (Contract.Trade.VetMemberId != UserContext.CurrentUser.Id)
            {
                return NotFound();
            }

            if (Contract.ContractStatus != ContractStatus.Suggestion)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}

            //DbContext.Attach(Contract).State = EntityState.Modified;

            var entity = DbContext.Contracts
                .Include(c=>c.VetMember)
                .Include(c => c.Trade.VetMember)
                .FirstOrDefault(c=>c.Id == Contract.Id);
            var trade = DbContext.Trades
                .Include(c=>c.VetMember)
                .FirstOrDefault(c => c.Id == entity.TradeId);
            //await TryUpdateModelAsync(entity, nameof(Contract));

            //TODO:契約を結ぶためのコード
            

            var escrowUser = DbContext.VetMembers.First(c => c.MemberType == MemberType.Escrow);
            var escrowSendUser = trade.Direction == Direction.Buy ?
                                      DbContext.VetMembers.Find(trade.VetMemberId) :
                                      DbContext.VetMembers.Find(entity.VetMemberId);

            var restAmmount = CoreService.CalcAmount(escrowSendUser);

            if(restAmmount < entity.Reword)
            {
                IsInsufficientFunds = true;

                return await OnGetAsync(Contract.Id);
            }



            entity.ContractStatus = ContractStatus.Working;
            entity.EscrowTransaction = new CoinTransaction
            {
                RecivedVetMemberId = escrowUser.Id,
                Amount=entity.Reword,
                SendeVetMemberId = escrowSendUser.Id,
                Text = $"{trade.Title } 代金 {entity.Reword}{SiteContext.CurrenryUnit}",       
                TransactionType = CoinTransactionType.Contract
            };

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

            await SendMessages(entity, trade.VetMember, entity.VetMember);

            return RedirectToPage("./Index", new { contractId = Contract.Id });
        }

        private bool ContractExists(int id)
        {
            return DbContext.Contracts.Any(e => e.Id == id);
        }

        private async Task SendMessages(Contract contract, VetMember sender, VetMember reciver)
        {
            //var stakeHolders = await GetStakeHolders(contract.Id);
            var trade = DbContext.Contracts.AsQueryable().Where(c => c.Id == contract.Id).Select(c => c.Trade).First();


//            var dmMessage = $@"提案が受理され契約されました。確認してください。
//タイトル:{trade.Title}
//契約主:{sender.Name}
//URL:https://vetcoin.azurewebsites.net/Trades/Contracts?contractId={contract.Id}
//{contract.AgreementContent}";
            var messageTargets = new[] { reciver };


            Discord.EmbedBuilder builder = new Discord.EmbedBuilder();
            builder.WithTitle(trade.Title)
            .WithAuthor(sender.Name, sender.GetAvaterIconUrl(), sender.GetMemberPageUrl(SiteContext.SiteBaseUrl))
            .WithUrl($"https://vetcoin.azurewebsites.net/Trades/Contracts?contractId={contract.Id}")
                .AddField("アクション", "提案が受理され契約されました。");
                //.AddField("メッセージ内容", message);



//#if DEBUG
//            messageTargets = new[] { sender };
//#endif

            await CoreService.SendDirectMessage(messageTargets, string.Empty, builder.Build());
        }

    }
}
