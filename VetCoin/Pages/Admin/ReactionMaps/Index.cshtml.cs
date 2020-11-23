using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VetCoin.Data;

namespace VetCoin.Pages.Admin.ReactionMaps
{
    public class IndexModel : PageModel
    {
        private readonly VetCoin.Data.ApplicationDbContext DbContext;

        public string SearchKey { get; set; }

        public IndexModel(VetCoin.Data.ApplicationDbContext context)
        {
            DbContext = context;
        }

        public IQueryable<ReactionMap> ReactionMapQuery { get; set; }

        public void OnGet(string searchKey)
        {
            ReactionMapQuery = DbContext.ReactionMaps.AsQueryable();

            if(!string.IsNullOrEmpty(searchKey))
        {
                //Todo:SearchImpl
                //ReactionMapQuery = ReactionMapQuery
                //    .Where(c => true);
            }

        }
    }
}
