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
    public class IndexModel : PageModel
    {
        private readonly VetCoin.Data.ApplicationDbContext DbContext;

        public string SearchKey { get; set; }
        public UserContext UserContext { get; set; }

        public IndexModel(ApplicationDbContext context, CoreService coreService)
        {
            DbContext = context;
            CoreService = coreService;
        }

        public IQueryable<Donation> DonationQuery { get; set; }
        public CoreService CoreService { get; }

        public void OnGet(string searchKey)
        {
            UserContext = CoreService.GetUserContext();
            DonationQuery = DbContext.Donations
                .Include(d => d.VetMember)
                .Include(c=>c.Doners)
                .Where(c=>c.DonationState != DonationState.Cancel)
                .AsQueryable();

            if(!string.IsNullOrEmpty(searchKey))
        {
                //Todo:SearchImpl
                //DonationQuery = DonationQuery
                //    .Where(c => true);
            }

        }
    }
}
