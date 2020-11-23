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
    public class SuggestionRegectModel : PageModel
    {
        private readonly VetCoin.Data.ApplicationDbContext DbContext;

        public SuggestionRegectModel(VetCoin.Data.ApplicationDbContext context, CoreService coreService)
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
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //DbContext.Attach(Contract).State = EntityState.Modified;

            var entity = DbContext.Contracts.Find(Contract.Id);
            //var trade = DbContext.Trades.Find(entity.TradeId);
            
            entity.ContractStatus = ContractStatus.Canceled;

            //var escrowUser = DbContext.VetMembers.First(c => c.MemberType == MemberType.Escrow);
            //var escrowTargetUser = trade.Direction == Direction.Buy ?
            //                          DbContext.VetMembers.Find(trade.VetMemberId) :
            //                          DbContext.VetMembers.Find(entity.VetMemberId);

            //entity.EscrowTransaction = new CoinTransaction
            //{
            //    RecivedVetMemberId = escrowUser.Id,
            //    Amount = entity.Reword,
            //    SendeVetMemberId = escrowTargetUser.Id,
            //    Text = $"{trade.Title }  {entity.Reword}VEC",
            //};

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
