using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VetCoin.Data;
using VetCoin.Services;

namespace VetCoin.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly CoreService CoreService;

        public UserContext UserContext { get; set; }

        public IQueryable<CoinTransaction> Transactions { get; set; }

        public IndexModel(ILogger<IndexModel> logger, CoreService coreService)
        {
            _logger = logger;
            CoreService = coreService;
        }

        public IActionResult OnGet()
        {
            if(User.Identity.IsAuthenticated)
            {
                UserContext = CoreService.GetUserContext();
                if (UserContext == null)
                {
                    return RedirectToPage("SignOut");
                }

                Transactions = CoreService
                                    .GetCoinTransactionQuery(UserContext.CurrentUser)
                                    .Include(c=>c.RecivedVetMember)
                                    .Include(c => c.SendVetMember)
                                    .OrderByDescending(c => c.CreateDate)
                                    .Take(20);
            }

            return Page();
        }
    }
}
