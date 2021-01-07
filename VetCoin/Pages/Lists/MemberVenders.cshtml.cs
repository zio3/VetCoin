using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VetCoin.Data;
using VetCoin.Data.VenerEntityes;

namespace VetCoin.Pages.Lists
{
    public class MemberVendersModel : PageModel
    {
        private readonly VetCoin.Data.ApplicationDbContext DbContext;

        public string SearchKey { get; set; }


        [BindProperty(SupportsGet = true)]
        public int MemberId { get; set; }

        public VetMember VetMember { get; set; }

        public MemberVendersModel(VetCoin.Data.ApplicationDbContext context)
        {
            DbContext = context;
        }

        public IQueryable<Vender> VenderQuery { get; set; }

        public void OnGet(string searchKey)
        {
            VetMember = DbContext.VetMembers.Find(MemberId);

            VenderQuery = DbContext.Venders
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
