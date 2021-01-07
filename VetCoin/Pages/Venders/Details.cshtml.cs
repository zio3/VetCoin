using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VetCoin.Codes;
using VetCoin.Data;
using VetCoin.Data.VenerEntityes;
using VetCoin.Services;
using VetCoin.Services.Chat;

namespace VetCoin.Pages.Venders
{
    public class DetailsModel : PageModel
    {
        private readonly VetCoin.Data.ApplicationDbContext DbContext;

        public DetailsModel(ApplicationDbContext context, CoreService coreService, DiscordService discordService, SiteContext siteContext)
        {
            DbContext = context;
            CoreService = coreService;
            DiscordService = discordService;
            SiteContext = siteContext;
        }

        public Vender Vender { get; set; }

        public bool IsOwner { get; set; }

        public bool IsVoted { get; set; }

        public int VoteCount { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Vender = await DbContext.Venders
                .Include(v => v.VetMember)
                .Include(c=>c.VenderMessages)
                    .ThenInclude(c=>c.VetMember)
                .Include(c => c.VenderSales)
                    .ThenInclude(c => c.VetMember)
                .FirstOrDefaultAsync(m => m.Id == id);

            var userContext = CoreService.GetUserContext();
            IsOwner = Vender.VetMemberId == userContext.CurrentUser.Id;

            if (Vender == null)
            {
                return NotFound();
            }

            VoteCount = await DbContext.VenderLikeVotes
                .AsQueryable()
                .CountAsync(c => c.VenderId == id);
            
            IsVoted = await DbContext.VenderLikeVotes
                            .AsQueryable()
                            .AnyAsync(c => c.VenderId == id && c.VetMemberId == userContext.CurrentUser.Id);

            return Page();
        }


        public async Task<IActionResult> OnPostAsync(int id, string mode)
        {
            switch (mode)
            {
                case "PostMessage":
                    return await PostMessageImpl(id);
                case "Buy":
                    return await Buy(id);
                default:
                    break;
            }
            return NotFound();
        }


        private async Task<IActionResult> PostMessageImpl(int id)
        {
            var vender
                = await DbContext.Venders
                    .Include(c=>c.VenderMessages)
                      .ThenInclude(c=>c.VetMember)
                    .FirstOrDefaultAsync(c=>c.Id == id);

            

            if (vender == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(PostMessage))
            {
                var userContext = CoreService.GetUserContext();
                var vm = new VenderMessage
                {
                    Message = PostMessage,
                    VenderId = vender.Id,
                    VetMemberId = userContext.CurrentUser.Id
                };

                DbContext.VenderMessages.Add(vm);
                await DbContext.SaveChangesAsync();
                await SendMessages(vender, userContext.CurrentUser, PostMessage);
                PostMessage = null;
            }


            return RedirectToPage("Details", new { id = id });
        }

        private async Task SendMessages(Vender vender, VetMember sender, string message)
        {
            var senderIsOwner = vender.VetMemberId == sender.Id;

            var messageTargets =
                senderIsOwner ? DbContext.VenderMessages
                    .AsQueryable()
                    .Where(c => c.VenderId == vender.Id)
                    .Select(c => c.VetMember).Distinct().ToArray()
                            : new[] { vender.VetMember };


            Discord.EmbedBuilder builder = new Discord.EmbedBuilder();
            builder.WithTitle($"{ vender.Title} 【メッセージ】")
                .WithAuthor(sender.Name, sender.GetAvaterIconUrl(), sender.GetMemberPageUrl(SiteContext.SiteBaseUrl))
                .WithUrl($"{SiteContext.SiteBaseUrl}Venders/Details?id={vender.Id}")
                .AddField("内容", message);

            await CoreService.SendDirectMessage(messageTargets, string.Empty, builder.Build());
        }


        private async Task<IActionResult> Buy(int id)
        {
            var vender = await DbContext.Venders
                .Include(c=>c.VetMember)
                .FirstOrDefaultAsync(c=>c.Id == id);
            if (vender == null)
            {
                return NotFound();
            }

            var userContext = CoreService.GetUserContext();
            var escrowUser = DbContext.VetMembers.FirstOrDefault(c => c.MemberType == MemberType.Escrow);

            if (Amount <= 0)
            {
                IsBuyError = true;
                ErrorMessage = "数値の入力が必要です";
                return await OnGetAsync(id);
            }

            var coinTransaction = new CoinTransaction
            {
                SendeVetMemberId = userContext.CurrentUser.Id,
                RecivedVetMemberId = escrowUser.Id,
                Amount = Amount,
                Text = $"{vender.Title} にて購入 {Amount}{SiteContext.CurrenryUnit} {BuyMessage}",
                TransactionType = CoinTransactionType.Vender
            };

            var venderSale = new VenderSale
            {
                Amount = Amount,
                Message = BuyMessage,
                VenderId = vender.Id,
                VetMemberId = userContext.CurrentUser.Id,
            };

            DbContext.VenderSales.Add(venderSale);
            DbContext.CoinTransactions.Add(coinTransaction);

            await DbContext.SaveChangesAsync();

            //全体への周知
            await WebHookNotification(userContext,vender, venderSale);

            if(vender.IsSalesNotification)
            {
                await SalesNotification(userContext, vender, venderSale);
            }

            if (vender.HasDeliveryMessage)
            {
                await DeliveryMessage(userContext, vender, venderSale);
            }




            return RedirectToPage("Details", new { id = id });
        }

        private async Task WebHookNotification(UserContext userContext, Vender vender, VenderSale venderSale)
        {

            var fields = new List<DiscordService.DiscordEmbed.Field>();


            fields.Add(new DiscordService.DiscordEmbed.Field
            {
                name = "金額",
                value = $"{venderSale.Amount}{SiteContext.CurrenryUnit}",
                inline = false
            });

            if (!string.IsNullOrWhiteSpace(venderSale.Message))
            {
                fields.Add(new DiscordService.DiscordEmbed.Field
                {
                    name = "メッセージ",
                    value = venderSale.Message,
                    inline = false
                });
            }

            await DiscordService.SendMessage(DiscordService.Channel.VenderNotification, string.Empty, new DiscordService.DiscordEmbed
            {
                title = $"{vender.Title} 【購入】",
                url = $"{SiteContext.SiteBaseUrl}Venders/Details?id={vender.Id}",
                author = new DiscordService.DiscordEmbed.Author
                {
                    url = userContext.CurrentUser.GetMemberPageUrl(SiteContext.SiteBaseUrl),
                    icon_url = userContext.CurrentUser.GetAvaterIconUrl(),
                    name = userContext.CurrentUser.Name
                },
                fields = fields.ToArray(),
            });
        }

        private async Task SalesNotification(UserContext userContext, Vender vender, VenderSale venderSale)
        {
            var builder = new Discord.EmbedBuilder();
            builder
                .WithTitle($"{vender.Title} 【購入】")
                .WithUrl($"{SiteContext.SiteBaseUrl}Venders/Details?id={vender.Id}")
                .WithAuthor(
                    userContext.CurrentUser.Name,
                    userContext.CurrentUser.GetAvaterIconUrl(),
                    userContext.CurrentUser.GetMemberPageUrl(SiteContext.SiteBaseUrl)
                )
                .AddField("金額", $"{venderSale.Amount}{SiteContext.CurrenryUnit}", false);

            if(!string.IsNullOrEmpty(venderSale.Message))
            {
                builder
                .AddField("メッセージ", venderSale.Message, false);
            }

                

            var messageTargets = new[] { vender.VetMember };
            await CoreService.SendDirectMessage(messageTargets, string.Empty, builder.Build());
        }

        private async Task DeliveryMessage(UserContext userContext, Vender vender, VenderSale venderSale)
        {
            var builder = new Discord.EmbedBuilder();
            builder
                .WithTitle($"{vender.Title} 【購入】")
                .WithUrl($"{SiteContext.SiteBaseUrl}Venders/Details?id={vender.Id}")
                .WithAuthor(
                    userContext.CurrentUser.Name,
                    userContext.CurrentUser.GetAvaterIconUrl(),
                    userContext.CurrentUser.GetMemberPageUrl(SiteContext.SiteBaseUrl)
                )
                .AddField("金額", $"{venderSale.Amount}{SiteContext.CurrenryUnit}", false);
                //.AddField("メッセージ", venderSale.Message, false);


            if (!string.IsNullOrEmpty(venderSale.Message))
            {
                builder
                .AddField("メッセージ", venderSale.Message, false);
            }

            //if (!string.IsNullOrEmpty(vender.DeliveryMessage))
            //{
            //    builder.WithDescription(vender.DeliveryMessage);
            //}
            //else
            //{
            //    builder.WithDescription("[配信メッセージが未設定です]");
            //}  
            string message = null;
            if(!string.IsNullOrEmpty(vender.DeliveryMessage))
            {
                message = $@"販売者からのメッセージです
----------------------------
{vender.DeliveryMessage}";

            }
            else
            {

            }


            var messageTargets = new[] { userContext.CurrentUser };
            await CoreService.SendDirectMessage(messageTargets, message, builder.Build());


            await Task.Yield();
        }


        public int MyProperty { get; set; }
        public CoreService CoreService { get; }
        public DiscordService DiscordService { get; }
        public SiteContext SiteContext { get; }


        [BindProperty]
        public string PostMessage { get; set; }

        [BindProperty]
        public string BuyMessage { get; set; }

        [BindProperty]
        public int Amount { get; set; }

        public bool IsBuyError { get; set; }

        public string ErrorMessage { get; set; }
    }
}
