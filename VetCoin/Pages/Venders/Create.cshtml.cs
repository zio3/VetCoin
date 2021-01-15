using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using VetCoin.Codes;
using VetCoin.Data;
using VetCoin.Data.VenerEntityes;
using VetCoin.Services;
using VetCoin.Services.Chat;

namespace VetCoin.Pages.Venders
{
    public class CreateModel : PageModel
    {
        private readonly VetCoin.Data.ApplicationDbContext DbContext;

        public CreateModel(ApplicationDbContext context, CoreService coreService, DiscordService discordService, SiteContext siteContext, StaticSettings staticSettings)
        {
            DbContext = context;
            CoreService = coreService;
            DiscordService = discordService;
            SiteContext = siteContext;
            StaticSettings = staticSettings;
        }

        public IActionResult OnGet()
        {
            Vender = new Vender
            {
                IsSalesNotification = true
            };

            return Page();
        }

        [BindProperty]
        public Vender Vender { get; set; }
        public CoreService CoreService { get; }
        public DiscordService DiscordService { get; }
        public SiteContext SiteContext { get; }
        public StaticSettings StaticSettings { get; }

        public async Task<IActionResult> OnPostAsync()
        {
            if(Vender.HasDeliveryMessage && string.IsNullOrWhiteSpace(Vender.DeliveryMessage))
            {
                ModelState.AddModelError("Vender.DeliveryMessage", "DMメッセージ内容を入力してください");
            }


            if (!ModelState.IsValid)
            {
                return Page();
            }


            var userContext = CoreService.GetUserContext();
            Vender.VetMemberId = userContext.CurrentUser.Id;

            DbContext.Venders.Add(Vender);
            await DbContext.SaveChangesAsync();

            await Notification(userContext);

            return RedirectToPage("./Index");
        }

        private async Task Notification(UserContext userContext)
        {
            await DiscordService.SendMessage(DiscordService.Channel.VenderNotification, string.Empty, new DiscordService.DiscordEmbed
            {
                title = Vender.Title,
                url = $"{StaticSettings.SiteBaseUrl}Venders/Details?id={Vender.Id}",
                author = new DiscordService.DiscordEmbed.Author
                {
                    url = userContext.CurrentUser.GetMemberPageUrl(StaticSettings.SiteBaseUrl),
                    icon_url = userContext.CurrentUser.GetAvaterIconUrl(),
                    name = userContext.CurrentUser.Name
                },
                fields = new DiscordService.DiscordEmbed.Field[]
             {
                                     new DiscordService.DiscordEmbed.Field
                    {
                        name = "標準金額",
                        value = $"{Vender.DefaultAmount }{ StaticSettings.CurrenryUnit}",
                        inline = false
                    },

                    new DiscordService.DiscordEmbed.Field
                    {
                        name = "内容",
                        value = Vender.Content,
                        inline = false
                    }
             },
            });
        }
    }
}