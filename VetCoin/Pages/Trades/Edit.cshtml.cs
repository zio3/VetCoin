using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VetCoin.Data;
using VetCoin.Services;

namespace VetCoin.Pages.Trades
{
    public class EditModel : PageModel
    {
        private readonly VetCoin.Data.ApplicationDbContext DbContext;
        private readonly CoreService CoreService;

        public EditModel(ApplicationDbContext context, CoreService coreService)
        {
            DbContext = context;
            CoreService = coreService;
        }

        [BindProperty]
        public Trade Trade { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Trade = await DbContext.Trades
                .Include(t => t.VetMember).FirstOrDefaultAsync(m => m.Id == id);

            var userContext = CoreService.GetUserContext();
            if(Trade.VetMemberId != userContext.CurrentUser.Id)
            {
                return NotFound();
            }

            if (Trade == null)
            {
                return NotFound();
            }
            ViewData["VetMemberId"] = new SelectList(DbContext.VetMembers, "Id", "Id");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var entity = DbContext.Trades.Find(Trade.Id);
            var userContext = CoreService.GetUserContext();
            if (entity.VetMemberId != userContext.CurrentUser.Id)
            {
                return NotFound();
            }

            await TryUpdateModelAsync(entity, nameof(Trade));

            try
            {
                await DbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TradeExists(entity.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool TradeExists(int id)
        {
            return DbContext.Trades.Any(e => e.Id == id);
        }
    }
}
