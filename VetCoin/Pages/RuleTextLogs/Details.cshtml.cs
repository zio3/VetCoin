using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VetCoin.Data;

namespace VetCoin.Pages.RuleTextLogs
{
    public class DetailsModel : PageModel
    {
        private readonly VetCoin.Data.ApplicationDbContext DbContext;

        public DetailsModel(VetCoin.Data.ApplicationDbContext context)
        {
            DbContext = context;
        }

        public RuleTextLog RuleTextLog { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            RuleTextLog = await DbContext.RuleTextLogs
                .AsQueryable()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (RuleTextLog == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
