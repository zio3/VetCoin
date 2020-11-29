using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VetCoin.Data;
using VetCoin.Services;

namespace VetCoin.Pages.Donations
{
    public class EditModel : PageModel
    {
        private readonly VetCoin.Data.ApplicationDbContext DbContext;

        public EditModel(ApplicationDbContext context, CoreService coreService)
        {
            DbContext = context;
            CoreService = coreService;
        }

        [BindProperty]
        public Donation Donation { get; set; }
        public CoreService CoreService { get; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uc = CoreService.GetUserContext();

            Donation = await DbContext.Donations
                .Include(d => d.VetMember).FirstOrDefaultAsync(m => m.Id == id);

            if (Donation == null)
            {
                return NotFound();
            }

            if(Donation.VetMemberId != uc.CurrentUser.Id)
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

            var entity = DbContext.Donations.Find(Donation.Id);

            var log = new DonationLog
            {
                Title = entity.Title,
                Content = entity.Content,
                DonationId = Donation.Id

            };
            DbContext.DonationLogs.Add(log);
            await TryUpdateModelAsync(entity, nameof(Donation));

            try
            {
                await DbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DonationExists(Donation.Id))
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

        private bool DonationExists(int id)
        {
            return DbContext.Donations.Any(e => e.Id == id);
        }
    }
}
