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
    public class MemberContractsModel : PageModel
    {
        private readonly VetCoin.Data.ApplicationDbContext DbContext;

        public string SearchKey { get; set; }

        [BindProperty(SupportsGet =true)]
        public int MemberId { get; set; }

        public VetMember VetMember { get; set; }

        public MemberContractsModel(VetCoin.Data.ApplicationDbContext context)
        {
            DbContext = context;
        }

        public IQueryable<Contract> ContractQuery { get; set; }

        public void OnGet(string searchKey)
        {
            VetMember = DbContext.VetMembers.Find(MemberId);
            ContractQuery = DbContext.Contracts
                //.Include(c => c.EscrowTransaction)
                .Include(c => c.Trade.VetMember)
                .Include(c => c.VetMember)
                .Where(c=>c.VetMemberId == MemberId)
                .AsQueryable();

            if(!string.IsNullOrEmpty(searchKey))
        {
                //Todo:SearchImpl
                //ContractQuery = ContractQuery
                //    .Where(c => true);
            }

        }
    }
}
