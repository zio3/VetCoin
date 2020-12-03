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
    public class MemberTransactionsModel : PageModel
    {
        private readonly VetCoin.Data.ApplicationDbContext DbContext;

        public string SearchKey { get; set; }

        [BindProperty(SupportsGet = true)]
        public int MemberId { get; set; }

        public VetMember VetMember { get; set; }


        public MemberTransactionsModel(VetCoin.Data.ApplicationDbContext context)
        {
            DbContext = context;
        }

        public IQueryable<CoinTransaction> CoinTransactionQuery { get; set; }

        public void OnGet(string searchKey)
        {
            VetMember = DbContext.VetMembers.Find(MemberId);
            CoinTransactionQuery = DbContext.CoinTransactions
                .Include(c => c.RecivedVetMember)
                .Include(c => c.SendVetMember)
                .Where(c=>c.SendeVetMemberId == MemberId || c.RecivedVetMemberId == MemberId)
                .AsQueryable();

            if(!string.IsNullOrEmpty(searchKey))
        {
                //Todo:SearchImpl
                //CoinTransactionQuery = CoinTransactionQuery
                //    .Where(c => true);
            }

        }
    }
}
