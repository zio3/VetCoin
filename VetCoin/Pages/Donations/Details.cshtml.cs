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
    public class DetailsModel : PageModel
    {
        private readonly VetCoin.Data.ApplicationDbContext DbContext;

        public DetailsModel(VetCoin.Data.ApplicationDbContext context, CoreService coreService)
        {
            DbContext = context;
            CoreService = coreService;
        }

        public Donation Donation { get; set; }

        [BindProperty]
        public string PostMessage { get; set; }

        [BindProperty]
        public int DonateAmount { get; set; }

        public bool IsOwner { get; set; }
        public CoreService CoreService { get; }

        public bool IsSuppotError { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Donation = await DbContext.Donations
                .AsQueryable()
                .Include(d => d.VetMember)
                .Include(d => d.DonationMessages)
                    .ThenInclude(c => c.VetMember)
                .Include(d => d.Doners)
                    .ThenInclude(c => c.VetMember)
                .FirstOrDefaultAsync(m => m.Id == id);

            var userContext = CoreService.GetUserContext();
            IsOwner = Donation.VetMemberId == userContext.CurrentUser.Id;

            if (Donation == null)
            {
                return NotFound();
            }
            return Page();
        }


        public async Task<IActionResult> OnPostAsync(int id, string mode)
        {
            switch (mode)
            {
                case "PostMessage":
                    return await PostMessageImpl(id);
                case "SupprtEntry":
                    return await SupprtEntry(id);
                default:

                    break;
            }
            return NotFound();
        }


        private async Task<IActionResult> PostMessageImpl(int id)
        {
            var donation = await DbContext.Donations.FindAsync(id);
            if (donation == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(PostMessage))
            {
                var userContext = CoreService.GetUserContext();
                var dm = new DonationMessage
                {
                    Message = PostMessage,
                    DonationId = donation.Id,
                    VetMemberId = userContext.CurrentUser.Id
                };

                DbContext.DonationMessages.Add(dm);
                await DbContext.SaveChangesAsync();


                //await SendMessages(trade, userContext.CurrentUser, postMessage);


                PostMessage = null;
            }
            return RedirectToPage("Details", new { id = id });
        }

        private async Task<IActionResult> SupprtEntry(int id)
        {
            var donation = await DbContext.Donations.FindAsync(id);
            if (donation == null)
            {
                return NotFound();
            }

            var userContext = CoreService.GetUserContext();
            var escrowUser = DbContext.VetMembers.FirstOrDefault(c => c.MemberType == MemberType.Escrow);

            if(userContext.Amount < DonateAmount)
            {
                IsSuppotError = true;
                return await OnGetAsync(id);
            }


            var coinTransaction = new CoinTransaction
            {
                SendeVetMemberId = userContext.CurrentUser.Id,
                RecivedVetMemberId = escrowUser.Id,
                Amount = DonateAmount,
                Text = $"{donation.Title} へ {PostMessage}",
                TransactionType = CoinTransactionType.Donate
            };

            var doner = new Doner
            {
                Amount = DonateAmount,
                Comment = PostMessage,
                DonationId = donation.Id,
                VetMemberId = userContext.CurrentUser.Id,
                CoinTransaction = coinTransaction                
            };

            DbContext.Doners.Add(doner);
            DbContext.CoinTransactions.Add(coinTransaction);

            await DbContext.SaveChangesAsync();

            return RedirectToPage("Details", new { id = id });

        }
    }
}
