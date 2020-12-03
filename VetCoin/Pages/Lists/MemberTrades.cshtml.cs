using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VetCoin.Data;

namespace VetCoin.Pages.Lists
{
    public class MemberTradesModel : PageModel
    {
        private readonly VetCoin.Data.ApplicationDbContext DbContext;

        public string SearchKey { get; set; }


        [BindProperty(SupportsGet = true)]
        public int MemberId { get; set; }

        public VetMember VetMember { get; set; }

        public MemberTradesModel(VetCoin.Data.ApplicationDbContext context)
        {
            DbContext = context;
        }

        public IQueryable<Trade> TradeQuery { get; set; }

        public void OnGet(string searchKey)
        {
            VetMember = DbContext.VetMembers.Find(MemberId);

            TradeQuery = DbContext.Trades
                .Include(t => t.VetMember)
                .Where(c => c.VetMemberId == MemberId)
                .AsQueryable();

            if(!string.IsNullOrEmpty(searchKey))
        {
                //Todo:SearchImpl
                //TradeQuery = TradeQuery
                //    .Where(c => true);
            }

        }
    }
}
