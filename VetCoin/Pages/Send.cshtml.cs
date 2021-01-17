using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using VetCoin.Data;
using VetCoin.Services;

namespace VetCoin.Pages
{
    public class SendModel : PageModel
    {
        public SendModel(CoreService coreService, StaticSettings staticSettings,ApplicationDbContext dbContext)
        {
            CoreService = coreService;
            StaticSettings = staticSettings;
            DbContext = dbContext;
        }

        public IEnumerable<SelectListItem> MembersDdl { get; set; }

        public UserContext UserContext { get; set; }

        [BindProperty]
        public long SendAmount { get; set; }

        [BindProperty]
        public string Message { get; set; }

        [BindProperty]
        public int ReciveVetMemberId { get; set; }
        public ApplicationDbContext DbContext { get; }
        public CoreService CoreService { get; }
        public StaticSettings StaticSettings { get; }

        public IActionResult OnGet(int? reciveVetMemberId)
        {
            UserContext = CoreService.GetUserContext();
            if(UserContext == null)
            {
                return NotFound();
            }
            MembersDdl = CoreService.GetOtherUserDdl(UserContext.CurrentUser);

            if(reciveVetMemberId.HasValue)
            {
                ReciveVetMemberId = reciveVetMemberId.Value;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(long sendAmount,string message ,int reciveVetMemberId)
        {
            UserContext = CoreService.GetUserContext();
            if (UserContext == null)
            {
                return NotFound();
            }

            if (sendAmount <= 0)
            {
                this.ModelState.AddModelError("sendAmount", "1以上の数を指定してください");
                return OnGet(null);
            }
            if (UserContext.Amount  < sendAmount)
            {
                this.ModelState.AddModelError("sendAmount", "残高が不足しています");
                return OnGet(null);
            }

            CoreService.AddTransaction(UserContext , sendAmount, message, reciveVetMemberId, CoinTransactionType.Transfer);
            await CoreService.SavechanesAsnc();

            var reciveMember = DbContext.VetMembers.Find(reciveVetMemberId);
            var lastAmount = CoreService.CalcAmount(reciveMember);

            await Notification(UserContext.CurrentUser, reciveMember, sendAmount,message, lastAmount);

            return RedirectToPage("/index");

        }

        private async Task Notification(VetMember from, VetMember to, long amount,string message, long totalAmount)
        {
            var builder = new Discord.EmbedBuilder();
            builder
                .WithTitle($"{from.Name} から{amount}{StaticSettings.CurrenryUnit} 送金されました")
                .WithAuthor(
                    from.Name,
                    from.GetAvaterIconUrl(),
                    from.GetMemberPageUrl(StaticSettings.SiteBaseUrl)
                )
                .AddField("金額", $"{amount}{StaticSettings.CurrenryUnit}", false)
                .AddField("残高", $"{totalAmount}{StaticSettings.CurrenryUnit}", false);

            if (!string.IsNullOrEmpty(message))
            {
                builder
                .AddField("メッセージ", message, false);
            }

            var messageTargets = new[] { to };
            await CoreService.SendDirectMessage(messageTargets, string.Empty, builder.Build());
        }

    }
}
