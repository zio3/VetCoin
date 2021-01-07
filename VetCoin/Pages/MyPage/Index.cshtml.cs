using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VetCoin.Data;
using VetCoin.Data.VenerEntityes;
using VetCoin.Services;

namespace VetCoin.Pages.MyPage
{
    public class IndexModel : PageModel
    {
        public IndexModel(CoreService coreService)
        {
            CoreService = coreService;
        }

        public CoreService CoreService { get; }

        public UserContext UserContext { get; set; }

        public VetMember VetMember { get; set; }

        public IEnumerable<Trade> Trades { get;set;}

        public IEnumerable<Trade> CommentedTrades { get; set; }


        public IEnumerable<Contract> Contracts { get; set; }

        public IEnumerable<Contract> WaitingContracts { get; set; }

        public IEnumerable<CoinTransaction> Transactions { get; set; }

        public IEnumerable<Vender> Venders { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            UserContext = CoreService.GetUserContext();
            if (UserContext == null)
            {
                return NotFound();
            }

            VetMember = UserContext.CurrentUser;

            Transactions = CoreService
                                .GetCoinTransactionQuery(UserContext.CurrentUser)
                                .Include(c => c.RecivedVetMember)
                                .Include(c => c.SendVetMember)
                                .OrderByDescending(c => c.CreateDate)
                                .Take(5);

            Trades = CoreService.DbContext
                            .Trades
                            .Include(c => c.VetMember)
                            .Include(c => c.TradeMessages)
                                .ThenInclude(c => c.VetMember)
                            .OrderByDescending(c => c.CreateDate)
                            .Where(c=>c.VetMemberId == UserContext.CurrentUser.Id)
                            .Take(5)
                            .ToArray();

            CommentedTrades = CoreService.DbContext
                            .Trades
                            .Include(c => c.VetMember)
                            .Include(c => c.TradeMessages)
                                .ThenInclude(c => c.VetMember)
                            .OrderByDescending(c => c.TradeMessages.Max(d => d.CreateDate))
                            .Where(c => 
                                c.TradeMessages.Any(d=>d.VetMemberId == UserContext.CurrentUser.Id) &&
                                c.VetMemberId != UserContext.CurrentUser.Id
                            )
                            .Take(5)
                            .ToArray();

            Contracts = await CoreService.DbContext
                                .Contracts
                                .Include(c => c.VetMember)
                                .Include(c => c.Trade.VetMember)
                                .Where(c => c.VetMemberId == UserContext.CurrentUser.Id)
                                .OrderByDescending(c => c.ContractMessages.Max(d => d.CreateDate))
                                .Take(5)
                                .ToArrayAsync();

            Venders = await CoreService.DbContext
                    .Venders
                    .Include(c => c.VetMember)
                    .Where(c => c.VetMemberId == UserContext.CurrentUser.Id)
                    .OrderByDescending(c => c.CreateDate)
                    .Take(5)
                    .ToArrayAsync();


            WaitingContracts = await CoreService.EnumWaitingContracts(VetMember);

            return Page();
        }
    }
}
