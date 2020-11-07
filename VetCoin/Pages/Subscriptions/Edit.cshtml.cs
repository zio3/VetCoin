using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VetCoin.Data;

namespace VetCoin.Pages.Subscriptions
{
    public class EditModel : PageModel
    {
        private readonly VetCoin.Data.ApplicationDbContext DbContext;

        public EditModel(VetCoin.Data.ApplicationDbContext context)
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //DbContext.Attach(Subscription).State = EntityState.Modified;

            var entity = DbContext.Subscriptions.Find(Subscription.Id);
            await TryUpdateModelAsync(entity, nameof(Subscription));


            try
            {
                await DbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubscriptionExists(Subscription.Id))
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

        private bool SubscriptionExists(int id)
        {
            return DbContext.Subscriptions.Any(e => e.Id == id);
        }
    }
}
