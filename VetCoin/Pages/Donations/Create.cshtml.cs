using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using VetCoin.Codes;
using VetCoin.Data;
using VetCoin.Services;
using VetCoin.Services.Chat;

namespace VetCoin.Pages.Donations
{
    public class CreateModel : PageModel
    {
        private readonly VetCoin.Data.ApplicationDbContext DbContext;

        public CreateModel(ApplicationDbContext context, CoreService coreService, DiscordService discordService, SiteContext siteContext)
        {
            DbContext = context;
            CoreService = coreService;
            DiscordService = discordService;
            SiteContext = siteContext;
        }

        public IActionResult OnGet()
        {

            return Page();
        }

        [BindProperty]
        public Donation Donation { get; set; }
        public CoreService CoreService { get; }
        public DiscordService DiscordService { get; }
        public SiteContext SiteContext { get; }
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

            await Notification(userContext);

            return RedirectToPage("./Index");
        }

        private async Task Notification(UserContext userContext)
        {
            await DiscordService.SendMessage(DiscordService.Channel.CrowdFundingNotification, string.Empty, new DiscordService.DiscordEmbed
            {
                title = Donation.Title,
                url = $"{SiteContext.SiteBaseUrl}Donations/Details?id={Donation.Id}",
                author = new DiscordService.DiscordEmbed.Author
                {
                    url = userContext.CurrentUser.GetMemberPageUrl(SiteContext.SiteBaseUrl),
                    icon_url = userContext.CurrentUser.GetAvaterIconUrl(),
                    name = userContext.CurrentUser.Name
                },
                fields = new DiscordService.DiscordEmbed.Field[]
             {
                    new DiscordService.DiscordEmbed.Field
                    {
                        name = "内容",
                        value = Donation.Content,
                        inline = false
                    }
             },
            });
        }
    }
}