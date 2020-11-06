using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VetCoin.Services;

namespace VetCoin.Pages.Account
{
    public class LoginModel : PageModel
    {
        public LoginModel(CoreService coreService)
        {
            CoreService = coreService;
        }

        public CoreService CoreService { get; }

        public IActionResult OnGet()
        {
            var url = CoreService.GetLoginPageUrl();

            return Redirect(url);

        }
    }
}
