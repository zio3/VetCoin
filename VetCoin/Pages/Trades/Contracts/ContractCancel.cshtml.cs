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
    public class ContractCancelModel : PageModel
    {
        private readonly VetCoin.Data.ApplicationDbContext DbContext;

        public ContractCancelModel(VetCoin.Data.ApplicationDbContext context, CoreService coreService)
        {
            DbContext = context;
            CoreService = coreService;
        }

        [BindProperty]
        public Contract Contract { get; set; }
        public Trade Trade { get; set; }

        public UserContext UserContext { get; set; }
        public CoreService CoreService { get; }

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

            var sellUser = Trade.Direction == Direction.Sell  ?
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

            if (sellUser.Id != UserContext.CurrentUser.Id)
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
                .Include(c=>c.EscrowTransaction)
                .First(c=>c.Id == Contract.Id);
            var trade = DbContext.Trades.Find(entity.TradeId);
            //await TryUpdateModelAsync(entity, nameof(Contract));

            //TODO:Œ_–ñ‚ðƒLƒƒƒ“ƒZƒ‹‚·‚é
            entity.ContractStatus = ContractStatus.Canceled;
            DbContext.CoinTransactions.Remove(entity.EscrowTransaction);
            entity.EscrowTransaction = null;

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

            return RedirectToPage("./Index", new { contractId = Contract.Id });
        }

        private bool ContractExists(int id)
        {
            return DbContext.Contracts.Any(e => e.Id == id);
        }
    }
}
