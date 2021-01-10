using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VetCoin.Data;

namespace VetCoin.Pages
{
    public class MembersModel : PageModel
    {
        private readonly VetCoin.Data.ApplicationDbContext DbContext;

        public string SearchKey { get; set; }

        public MembersModel(VetCoin.Data.ApplicationDbContext context)
        {
            DbContext = context;
        }

        //public IQueryable<VetMember> VetMemberQuery { get; set; }

        public IQueryable<MemberInfo> MemberInfoes { get; set; }

        public void OnGet(string searchKey)
        {
            MemberInfoes = DbContext.VetMembers
                .AsQueryable()
                .Where(c=>c.DiscordId != 0)
                .Select(c=>new MemberInfo
                {
                    Member = c,
                    Amount = c.RecivedTransactions.Sum(d=>((int?)d.Amount)??0) - (c.SendTransactions.Sum(d=>(int?)d.Amount) ?? 0) ,
                    SendAmount = c.SendTransactions.Sum(d => (int?)d.Amount) ?? 0,
                    ReciveAmount = c.RecivedTransactions.Sum(d => (int?)d.Amount) ?? 0,

                })
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchKey))
            {
                //Todo:SearchImpl
                MemberInfoes = MemberInfoes
                    .Where(c => c.Member.Name.Contains(searchKey));
            }
        }

        public class MemberInfo
        {
            public VetMember Member { get; set; }

            public int Amount { get; set; }

            public int SendAmount { get; set; }
            public int ReciveAmount { get; set; }
        }
    }
}
