using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VetCoin.Data;

namespace VetCoin.Pages.Trades
{
    public class DeleteModel : PageModel
    {
        private readonly VetCoin.Data.ApplicationDbContext DbContext;

        public DeleteModel(VetCoin.Data.ApplicationDbContext context)
        {
            DbContext = context;
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

            if (Trade == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Trade = await DbContext.Trades.FindAsync(id);

            if (Trade != null)
            {
                Trade.TradeStatus = TradeStatus.Cancel;
                await DbContext.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
