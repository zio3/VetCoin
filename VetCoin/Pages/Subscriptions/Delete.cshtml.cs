using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VetCoin.Data;

namespace VetCoin.Pages.Subscriptions
{
    public class DeleteModel : PageModel
    {
        private readonly VetCoin.Data.ApplicationDbContext DbContext;

        public DeleteModel(VetCoin.Data.ApplicationDbContext context)
        {
            DbContext = context;
        }

        [BindProperty]
        public Subscription Subscription { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Subscription = await DbContext.Subscriptions
                .AsQueryable()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Subscription == null)
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

            Subscription = await DbContext.Subscriptions.FindAsync(id);

            if (Subscription != null)
            {
                Subscription.SubscriptionStatus = SubscriptionStatus.Cancel;
                await DbContext.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
