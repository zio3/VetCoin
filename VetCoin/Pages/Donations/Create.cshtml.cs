using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using VetCoin.Data;
using VetCoin.Services;
using VetCoin.Services.Chat;

namespace VetCoin.Pages.Donations
{
    public class CreateModel : PageModel
    {
        private readonly VetCoin.Data.ApplicationDbContext DbContext;

        public CreateModel(ApplicationDbContext context, CoreService coreService, DiscordService discordService)
        {
            DbContext = context;
            CoreService = coreService;
            DiscordService = discordService;
        }

        public IActionResult OnGet()
        {

            return Page();
        }

        [BindProperty]
        public Donation Donation { get; set; }
        public CoreService CoreService { get; }
        public DiscordService DiscordService { get; }

        [BindProperty]
        [DisplayName("通知不要")]
        public bool IsSkipNotification { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var userContext = CoreService.GetUserContext();
            Donation.VetMemberId = userContext.CurrentUser.Id;

            DbContext.Donations.Add(Donation);
            await DbContext.SaveChangesAsync();

            if (!IsSkipNotification)
            {
                //TODO:通知
            }

            return RedirectToPage("./Index");
        }
    }
}