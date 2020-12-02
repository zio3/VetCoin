using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VetCoin.Data;
using VetCoin.Services;

namespace VetCoin.Pages.Trades.Contracts
{
    public class CreateModel : PageModel
    {
        private readonly VetCoin.Data.ApplicationDbContext DbContext;

        public CreateModel(VetCoin.Data.ApplicationDbContext context, CoreService coreService)
        {
            DbContext = context;
            CoreService = coreService;
        }


        public IActionResult OnGet(int tradeId)
        {
            //ViewData["TradeId"] = new SelectList(DbContext.Trades, "Id", "Id");
            //ViewData["VetMemberId"] = new SelectList(DbContext.VetMembers, "Id", "Id");

            Trade = DbContext.Trades
                .Include(c => c.VetMember)
                .FirstOrDefault(c => c.Id == tradeId);
            Contract = new Contract
            {

            };

            if(Trade.Reward.HasValue)
            {
                Contract.Reword = Trade.Reward.Value;
            }
            //Contract.AgreementContent = Trade.Content;
            Contract.DeliveryDate = Trade.DeliveryDate;
            return Page();
        }

        public Trade Trade { get; set; }


        [BindProperty]
        public Contract Contract { get; set; }
        public CoreService CoreService { get; }

        public async Task<IActionResult> OnPostAsync(int tradeId)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var userContext = CoreService.GetUserContext();
            Contract.VetMemberId = userContext.CurrentUser.Id;
            Contract.TradeId = tradeId;

            DbContext.Contracts.Add(Contract);
            await DbContext.SaveChangesAsync();

            await SendMessages(Contract, userContext.CurrentUser);

            return RedirectToPage("./Index", new { contractId = Contract.Id });
        }


        private async Task<VetMember[]> GetStakeHolders(int id)
        {
            var contractMember = DbContext.Contracts.AsQueryable().Where(c => c.Id == id).Select(c => c.VetMember);
            var tradeMember = DbContext.Contracts.AsQueryable().Where(c => c.Id == id).Select(c => c.Trade.VetMember);

            var stakeHolders = contractMember.Concat(tradeMember).Distinct();
            return await stakeHolders.ToArrayAsync();
        }

        private async Task SendMessages(Contract contract, VetMember sender)
        {
            var stakeHolders = await GetStakeHolders(contract.Id);
            var trade = DbContext.Contracts.AsQueryable().Where(c => c.Id == contract.Id).Select(c => c.Trade).First();

            EmbedBuilder builder = new EmbedBuilder();
            builder.WithTitle(trade.Title)
                .WithAuthor(sender.Name, sender.GetAvaterIconUrl(), sender.GetMemberPageUrl())
                .WithUrl($"https://vetcoin.azurewebsites.net/Trades/Contracts?contractId={contract.Id}")
                .AddField("アクション", "提案がありました")
                .AddField("報酬", contract.Reword,true)
                .AddField("納期", GetNotEmptyStr(contract.DeliveryDate), true)
                .AddField("提案内容", GetNotEmptyStr(contract.AgreementContent));

//            var dmMessage = $@"提案がありました。確認してください。
//タイトル:{trade.Title}
//提案主:{sender.Name}
//URL:https://vetcoin.azurewebsites.net/Trades/Contracts?contractId={contract.Id}
//{contract.AgreementContent}";

            var messageTargets = stakeHolders.Where(c => c.Id != sender.Id).ToArray();

            await CoreService.SendDirectMessage(messageTargets, string.Empty, builder.Build());
        }

        string GetNotEmptyStr(string src)
        {
            if (string.IsNullOrEmpty(src.Trim()))
            {
                src = "[未設定]";
            }
            return src;
        }
    }
}