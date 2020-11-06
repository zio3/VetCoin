using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VetCoin.Data;
using VetCoin.Services;

namespace VetCoin.Pages.Trades.Contracts
{
    public class SuggestionEditModel : PageModel
    {
        private readonly VetCoin.Data.ApplicationDbContext DbContext;

        public SuggestionEditModel(VetCoin.Data.ApplicationDbContext context,CoreService coreService)
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

            if(Contract.VetMemberId != UserContext.CurrentUser.Id)
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
            await TryUpdateModelAsync(entity, nameof(Contract));


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

            return RedirectToPage("./Index",new { contractId = Contract.Id });
        }

        private bool ContractExists(int id)
        {
            return DbContext.Contracts.Any(e => e.Id == id);
        }
    }
}
