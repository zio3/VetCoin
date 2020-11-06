using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VetCoin.Data;
using VetCoin.Services;

namespace VetCoin.Pages
{
    public class LogsModel : PageModel
    {

        public string SearchKey { get; set; }


        public LogsModel(CoreService coreService)
        {
            CoreService = coreService;
        }

        public CoreService CoreService { get; }

        public IQueryable<CoinTransaction> CoinTransactionQuery { get; set; }
        public UserContext UserContext { get; set; }

        public IActionResult OnGet(string searchKey)
        {
            UserContext = CoreService.GetUserContext();
            if (UserContext == null)
            {
                return NotFound();
            }


            CoinTransactionQuery = CoreService.GetCoinTransactionQuery(UserContext.CurrentUser)
                .Include(c => c.RecivedVetMember)
                .Include(c => c.SendVetMember)
                .OrderByDescending(c=>c.CreateDate)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchKey))
            {
                //Todo:SearchImpl
                CoinTransactionQuery = CoinTransactionQuery
                    .Where(c =>
                    c.Text.Contains(searchKey) ||
                    c.RecivedVetMember.Name.Contains(searchKey) ||
                    c.SendVetMember.Name.Contains(searchKey) );
            }

            return Page();

        }
    }
}
