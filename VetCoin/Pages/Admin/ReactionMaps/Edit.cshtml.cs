using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VetCoin.Codes;
using VetCoin.Data;

namespace VetCoin.Pages.Admin.ReactionMaps
{
    public class EditModel : PageModel
    {
        private readonly VetCoin.Data.ApplicationDbContext DbContext;

        public EditModel(VetCoin.Data.ApplicationDbContext context,SiteContext siteContext)
        {
            DbContext = context;
            SiteContext = siteContext;
        }

        [BindProperty]
        public ReactionMap ReactionMap { get; set; }
        public SiteContext SiteContext { get; }

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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //DbContext.Attach(ReactionMap).State = EntityState.Modified;

            var entity = DbContext.ReactionMaps.Find(ReactionMap.Id);
            await TryUpdateModelAsync(entity, nameof(ReactionMap));


            try
            {
                await DbContext.SaveChangesAsync();
                SiteContext.ClearReactionMap();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReactionMapExists(ReactionMap.Id))
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

        private bool ReactionMapExists(int id)
        {
            return DbContext.ReactionMaps.Any(e => e.Id == id);
        }
    }
}
