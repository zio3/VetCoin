using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VetCoin.Data;
using VetCoin.Data.VenerEntityes;
using VetCoin.Services;

namespace VetCoin.Pages.Donations
{
    public class SwitchModel : PageModel
    {
        private readonly VetCoin.Data.ApplicationDbContext DbContext;

        public SwitchModel(ApplicationDbContext context, CoreService coreService)
        {
            DbContext = context;
            CoreService = coreService;
        }

        [BindProperty]
        public Donation Donation { get; set; }
        public CoreService CoreService { get; }

        [BindProperty]
        public string mode{ get; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uc = CoreService.GetUserContext();

            Donation = await DbContext.Donations
                .Include(v => v.VetMember).FirstOrDefaultAsync(m => m.Id == id);

            if (Donation == null)
            {
                return NotFound();
            }

            if (Donation.VetMemberId != uc.CurrentUser.Id)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string mode)
        {
            var entity = DbContext.Donations.Find(Donation.Id);

            switch (mode)
            {
                case "Open":
                    entity.DonationState = DonationState.Open;
                    break;
                case "Close":
                    entity.DonationState = DonationState.Close;
                    break;
                case "Cancel":
                    entity.DonationState = DonationState.Cancel;
                    break;
                default:
                    break;
            }



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

            return RedirectToPage("./Details", new { id = Donation.Id });
        }

        private bool DonationExists(int id)
        {
            return DbContext.Donations.Any(e => e.Id == id);
        }
    }
}
