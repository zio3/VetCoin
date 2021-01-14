using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VetCoin.Data;
using VetCoin.Data.ExtApp;

namespace VetCoin.Pages.ExtApps.ExteralApplications
{
    public class DetailsModel : PageModel
    {
        private readonly VetCoin.Data.ApplicationDbContext DbContext;

        public DetailsModel(VetCoin.Data.ApplicationDbContext context)
        {
            DbContext = context;
        }

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
            return Page();
        }
    }
}
