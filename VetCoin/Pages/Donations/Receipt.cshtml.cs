using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VetCoin.Data;
using VetCoin.Services;

namespace VetCoin.Pages.Donations
{
    public class ReceiptModel : PageModel
    {
        public ReceiptModel(ApplicationDbContext dbContext, CoreService coreService)
        {
            DbContext = dbContext;
            CoreService = coreService;
        }

        public ApplicationDbContext DbContext { get; }
        public CoreService CoreService { get; }

        public Doner[] Doners { get; set; }

        [BindProperty(SupportsGet =true)]
        public int DonationId { get; set; }

        public async Task<IActionResult> OnGetAsync(int donationId)
        {
            var uc = CoreService.GetUserContext();
            var entity = DbContext.Donations.Find(donationId);

            if (entity.VetMemberId != uc.CurrentUser.Id)
            {
                return NotFound();
            }
            Doners = DbContext.Doners
                .Include(c=>c.VetMember)
                .AsQueryable()
                .Where(c => c.DonationId == donationId)
                .ToArray();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int donationId,string mode,int? donerId)
        {
            switch (mode)
            {
                case "reciptAll":
                    return await ReciptAll(donationId);
                case "recipt":
                    return await Recipt(donationId,donerId.Value);
                case "cancel":
                    return await Cancel(donationId, donerId.Value);
                default:
                    break;
            }
            return null;
        }

        public async Task<IActionResult> ReciptAll(int donationId)
        {
            var donation = await DbContext
                .Donations.FindAsync(donationId);

            var doners = DbContext.Doners
                .AsQueryable()
                .Include(c=>c.CoinTransaction)
                .Where(c => c.DonationId == donationId && c.DonerState == DonerState.Entry)
                .ToArray();

            foreach (var doner in doners)
            {
                doner.DonerState = DonerState.Repted;
                doner.CoinTransaction.RecivedVetMemberId = donation.VetMemberId;
            }

            await DbContext.SaveChangesAsync();

            return RedirectToPage("Receipt",new { donationId  = donationId });
        }
        public async Task<IActionResult> Recipt(int donationId, int donerId)
        {
            var donation = await DbContext
                .Donations.FindAsync(donationId);
            var doner = DbContext.Doners
                .AsQueryable()
                .Include(c => c.CoinTransaction)
                .Where(c => c.DonationId == donationId)
                .FirstOrDefault(c => c.Id == donerId);

            doner.DonerState = DonerState.Repted;
            doner.CoinTransaction.RecivedVetMemberId = donation.VetMemberId;

            await DbContext.SaveChangesAsync();

            return RedirectToPage("Receipt", new { donationId = donationId });
        }
        public async Task<IActionResult> Cancel(int donationId,int donerId)
        {
            var donation = await DbContext
                .Donations.FindAsync(donationId);
            var doner = DbContext.Doners
                .AsQueryable()
                .Include(c => c.CoinTransaction)
                .Where(c => c.DonationId == donationId)
                .FirstOrDefault(c => c.Id == donerId);

            
            doner.DonerState = DonerState.Cancel;
            doner.CoinTransactionId = null;
            DbContext.CoinTransactions.Remove(doner.CoinTransaction);

            await DbContext.SaveChangesAsync();

            return RedirectToPage("Receipt", new { donationId = donationId });
        }
    }
}
