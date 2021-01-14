using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using VetCoin.Data;
using VetCoin.Data.ExtApp;

namespace VetCoin.Pages.ExtApps.ExteralApplications
{
    public class CreateModel : PageModel
    {
        private readonly VetCoin.Data.ApplicationDbContext DbContext;

        public CreateModel(VetCoin.Data.ApplicationDbContext context)
        {
            DbContext = context;
        }

        public IActionResult OnGet()
        {
        ViewData["VetMemberId"] = new SelectList(DbContext.VetMembers, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public ExteralApplication ExteralApplication { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            DbContext.ExteralApplications.Add(ExteralApplication);
            await DbContext.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}