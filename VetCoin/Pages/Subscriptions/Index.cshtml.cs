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
    public class IndexModel : PageModel
    {
        private readonly VetCoin.Data.ApplicationDbContext DbContext;

        public string SearchKey { get; set; }

        public IndexModel(VetCoin.Data.ApplicationDbContext context)
        {
            DbContext = context;
        }

        public IQueryable<Subscription> SubscriptionQuery { get; set; }

        public void OnGet(string searchKey)
        {
            SubscriptionQuery = DbContext.Subscriptions
                .Include(c=>c.VetMember)
                .Where(c=>c.SubscriptionStatus == SubscriptionStatus.Open)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchKey))
            {
                //Todo:SearchImpl
                //SubscriptionQuery = SubscriptionQuery
                //    .Where(c => true);
            }

        }
    }
}
