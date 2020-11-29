using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VetCoin.Data;

namespace VetCoin.Pages.Donations
{
    public class EditLogModel : PageModel
    {
        public EditLogModel(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public IEnumerable<DonationLog> DonationLogs { get; set; }
        public ApplicationDbContext DbContext { get; }

        public void OnGet(int donationId)
        {
            DonationLogs =  DbContext.DonationLogs
                .AsQueryable()
                .Where(c => c.DonationId == donationId)
                .OrderByDescending(c=>c.CreateDate)
                .ToArray();
        }
    }
}
