using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VetCoin.Data;

namespace VetCoin.Pages.Admin
{
    public class ClearItemsModel : PageModel
    {
        public ClearItemsModel(ApplicationDbContext applicationDbContext)
        {
            ApplicationDbContext = applicationDbContext;
        }

        public ApplicationDbContext ApplicationDbContext { get; }

        public IActionResult OnGet()
        {
            if(User.Identity.Name != "zio3")
            {
                return NotFound();
            }


            return Page();
        }

        public void OnPost()
        {

            ApplicationDbContext.Trades.RemoveRange(ApplicationDbContext.Trades.ToArray());

            ApplicationDbContext.CoinTransactions.RemoveRange(ApplicationDbContext.CoinTransactions.ToArray());

            ApplicationDbContext.SaveChanges();
        }
    }
}
