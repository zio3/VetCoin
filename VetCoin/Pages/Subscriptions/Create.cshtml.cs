using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using VetCoin.Data;
using VetCoin.Services;
using VetCoin.Services.Chat;

namespace VetCoin.Pages.Subscriptions
{
    public class CreateModel : PageModel
    {
        private readonly VetCoin.Data.ApplicationDbContext DbContext;

        public CreateModel(ApplicationDbContext context, CoreService coreService, DiscordService discordService)
        {
            DbContext = context;
            CoreService = coreService;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Subscription Subscription { get; set; }
        public CoreService CoreService { get; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var userContext = CoreService.GetUserContext();
            Subscription.VetMemberId = userContext.CurrentUser.Id;

            DbContext.Subscriptions.Add(Subscription);
            await DbContext.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}