using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VetCoin.Codes;
using VetCoin.Data;
using VetCoin.Services;
using VetCoin.Services.Chat;

namespace VetCoin.Pages.Donations
{
    public class DetailsModel : PageModel
    {
        private readonly VetCoin.Data.ApplicationDbContext DbContext;

        public DetailsModel(ApplicationDbContext context, CoreService coreService, DiscordService discordService, SiteContext siteContext, StaticSettings staticSettings)
        {
            DbContext = context;
            CoreService = coreService;
            DiscordService = discordService;
            SiteContext = siteContext;
            StaticSettings = staticSettings;
        }
        public bool IsVoted { get; set; }

        public int VoteCount { get; set; }

        public Donation Donation { get; set; }

        [BindProperty]
        public string PostMessage { get; set; }

        [BindProperty]
        public int DonateAmount { get; set; }

        public bool IsOwner { get; set; }
        public CoreService CoreService { get; }
        public DiscordService DiscordService { get; }
        public SiteContext SiteContext { get; }
        public StaticSettings StaticSettings { get; }
        public bool IsSuppotError { get; set; }

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Donation = await DbContext.Donations
                .AsQueryable()
                .Include(d => d.VetMember)
                .Include(d => d.DonationMessages)
                    .ThenInclude(c => c.VetMember)
                .Include(d => d.Doners)
                    .ThenInclude(c => c.VetMember)
                .FirstOrDefaultAsync(m => m.Id == id);

            var userContext = CoreService.GetUserContext();
            IsOwner = Donation.VetMemberId == userContext.CurrentUser.Id;

            if (Donation == null)
            {
                return NotFound();
            }

            VoteCount = await DbContext.DonationLikeVotes
                .AsQueryable()
                .CountAsync(c => c.DonationId == id);
            IsVoted = await DbContext.DonationLikeVotes
                            .AsQueryable()
                .AnyAsync(c => c.DonationId == id && c.VetMemberId == userContext.CurrentUser.Id);


            return Page();
        }


        public async Task<IActionResult> OnPostAsync(int id, string mode)
        {
            switch (mode)
            {
                case "PostMessage":
                    return await PostMessageImpl(id);
                case "SupprtEntry":
                    return await SupprtEntry(id);
                default:
                    break;
            }
            return NotFound();
        }


        private async Task<IActionResult> PostMessageImpl(int id)
        {
            var donation = await DbContext.Donations.FindAsync(id);
            if (donation == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(PostMessage))
            {
                var userContext = CoreService.GetUserContext();
                var dm = new DonationMessage
                {
                    Message = PostMessage,
                    DonationId = donation.Id,
                    VetMemberId = userContext.CurrentUser.Id
                };

                DbContext.DonationMessages.Add(dm);
                await DbContext.SaveChangesAsync();


                //await SendMessages(trade, userContext.CurrentUser, postMessage);


                PostMessage = null;
            }
            return RedirectToPage("Details", new { id = id });
        }

        private async Task<IActionResult> SupprtEntry(int id)
        {
            var donation = await DbContext.Donations.FindAsync(id);
            if (donation == null)
            {
                return NotFound();
            }

            var userContext = CoreService.GetUserContext();
            var escrowUser = DbContext.VetMembers.FirstOrDefault(c => c.MemberType == MemberType.Escrow);

            if (DonateAmount <= 0)
            {
                IsSuppotError = true;
                ErrorMessage = "数値の入力が必要です";
                return await OnGetAsync(id);
            }

            if (userContext.Amount < DonateAmount)
            {
                IsSuppotError = true;
                ErrorMessage = "残高不足です";
                return await OnGetAsync(id);
            }
            var coinTransaction = new CoinTransaction
            {
                SendeVetMemberId = userContext.CurrentUser.Id,
                RecivedVetMemberId = escrowUser.Id,
                Amount = DonateAmount,
                Text = $"{donation.Title} へ {PostMessage}",
                TransactionType = CoinTransactionType.Donate
            };

            var doner = new Doner
            {
                Amount = DonateAmount,
                Comment = PostMessage,
                DonationId = donation.Id,
                VetMemberId = userContext.CurrentUser.Id,
                CoinTransaction = coinTransaction                
            };

            DbContext.Doners.Add(doner);
            DbContext.CoinTransactions.Add(coinTransaction);

            await DbContext.SaveChangesAsync();

            await Notification(userContext,donation,doner);

            return RedirectToPage("Details", new { id = id });
        }

        private async Task Notification(UserContext userContext,Donation donation, Doner doner)
        {
            var fields = new List<DiscordService.DiscordEmbed.Field>();

            fields.Add(new DiscordService.DiscordEmbed.Field
            {
                name = "金額",
                value = $"{doner.Amount}{StaticSettings.CurrenryUnit}",
                inline = false
            });

            if (!string.IsNullOrWhiteSpace(doner.Comment))
            {
                fields.Add(new DiscordService.DiscordEmbed.Field
                {
                    name = "メッセージ",
                    value = doner.Comment,
                    inline = false
                });
            }

            var total = DbContext.Doners.AsQueryable().Where(c => c.DonationId == donation.Id)
                .Where(c=>c.DonerState != DonerState.Cancel)
                .Where(c=>c.VetMemberId != donation.VetMemberId)
                .Sum(c => c.Amount);

            fields.Add(new DiscordService.DiscordEmbed.Field
            {
                name = "支援総額",
                value = $"{total}{StaticSettings.CurrenryUnit}",
                inline = false
            });


            await DiscordService.SendMessage(DiscordService.Channel.CrowdFundingNotification, string.Empty, new DiscordService.DiscordEmbed
            {
                title = donation.Title,
                url = $"{StaticSettings.SiteBaseUrl}Donations/Details?id={donation.Id}",
                author = new DiscordService.DiscordEmbed.Author
                {
                    url = userContext.CurrentUser.GetMemberPageUrl(StaticSettings.SiteBaseUrl),
                    icon_url = userContext.CurrentUser.GetAvaterIconUrl(),
                    name = userContext.CurrentUser.Name
                },
                fields = fields.ToArray()
            });
        }
    }
}
