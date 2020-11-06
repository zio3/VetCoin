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

        public IQueryable<VetMember> VetMemberQuery { get; set; }

        public void OnGet(string searchKey)
        {
            VetMemberQuery = DbContext.VetMembers
                .AsQueryable()
                .Where(c=>c.DiscordId != 0)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchKey))
            {
                //Todo:SearchImpl
                VetMemberQuery = VetMemberQuery
                    .Where(c => c.Name.Contains(searchKey));
            }

        }
    }
}
