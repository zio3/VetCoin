using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VetCoin.Data;

namespace VetCoin.Pages.Admin.ReactionMaps
{
    public class DeleteModel : PageModel
    {
        private readonly VetCoin.Data.ApplicationDbContext DbContext;

        public DeleteModel(VetCoin.Data.ApplicationDbContext context)
        {
            DbContext = context;
        }

        [BindProperty]
        public ReactionMap ReactionMap { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ReactionMap = await DbContext.ReactionMaps
                .AsQueryable()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ReactionMap == null)
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

            ReactionMap = await DbContext.ReactionMaps.FindAsync(id);

            if (ReactionMap != null)
            {
                DbContext.ReactionMaps.Remove(ReactionMap);
                await DbContext.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
