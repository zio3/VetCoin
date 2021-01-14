using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VetCoin.Data;
using VetCoin.Data.ExtApp;

namespace VetCoin.Pages.ExtApps.ExteralApplications
{
    public class IndexModel : PageModel
    {
        private readonly VetCoin.Data.ApplicationDbContext DbContext;

        public string SearchKey { get; set; }

        public IndexModel(VetCoin.Data.ApplicationDbContext context)
        {
            DbContext = context;
        }

        public IQueryable<ExteralApplication> ExteralApplicationQuery { get; set; }

        public void OnGet(string searchKey)
        {
            ExteralApplicationQuery = DbContext.ExteralApplications
                .Include(e => e.VetMember).AsQueryable();

            if(!string.IsNullOrEmpty(searchKey))
        {
                //Todo:SearchImpl
                //ExteralApplicationQuery = ExteralApplicationQuery
                //    .Where(c => true);
            }

        }
    }
}
