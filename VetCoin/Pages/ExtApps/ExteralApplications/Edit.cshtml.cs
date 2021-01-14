using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VetCoin.Data;
using VetCoin.Data.ExtApp;

namespace VetCoin.Pages.ExtApps.ExteralApplications
{
    public class EditModel : PageModel
    {
        private readonly VetCoin.Data.ApplicationDbContext DbContext;

        public EditModel(VetCoin.Data.ApplicationDbContext context)
        {
            DbContext = context;
        }

        [BindProperty]
        public ExteralApplication ExteralApplication { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ExteralApplication = await DbContext.ExteralApplications
                .Include(e => e.VetMember).FirstOrDefaultAsync(m => m.Id == id);

            if (ExteralApplication == null)
            {
                return NotFound();
            }
            ViewData["VetMemberId"] = new SelectList(DbContext.VetMembers, "Id", "Name");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //DbContext.Attach(ExteralApplication).State = EntityState.Modified;

            var entity = DbContext.ExteralApplications.Find(ExteralApplication.Id);
            await TryUpdateModelAsync(entity, nameof(ExteralApplication));


            try
            {
                await DbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExteralApplicationExists(ExteralApplication.Id))
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

        private bool ExteralApplicationExists(Guid id)
        {
            return DbContext.ExteralApplications.Any(e => e.Id == id);
        }
    }
}
