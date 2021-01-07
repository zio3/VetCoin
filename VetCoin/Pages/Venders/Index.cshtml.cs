using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VetCoin.Data;
using VetCoin.Data.VenerEntityes;
using VetCoin.Services;

namespace VetCoin.Pages.Venders
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

        public IQueryable<Vender> VenderQuery { get; set; }
        public CoreService CoreService { get; }

        public void OnGet(string searchKey)
        {
            VenderQuery = DbContext.Venders
                .Include(v => v.VetMember)
                .AsQueryable();

            VenderQuery = VenderQuery.Where(c => !c.IsClosed);

            UserContext = CoreService.GetUserContext();

            if (!string.IsNullOrEmpty(searchKey))
        {
                //Todo:SearchImpl
                //VenderQuery = VenderQuery
                //    .Where(c => true);
            }

        }
    }
}
