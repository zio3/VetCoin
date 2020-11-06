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
        public SendModel(CoreService coreService)
        {
            CoreService = coreService;
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
                this.ModelState.AddModelError("sendAmount", "1ˆÈã‚Ì”‚ðŽw’è‚µ‚Ä‚­‚¾‚³‚¢");
                return OnGet(null);
            }
            if (UserContext.Amount  < sendAmount)
            {
                this.ModelState.AddModelError("sendAmount", "Žc‚‚ª•s‘«‚µ‚Ä‚¢‚Ü‚·");
                return OnGet(null);
            }

            CoreService.AddTransaction(UserContext , sendAmount, message, reciveVetMemberId, CoinTransactionType.Transfer);
            await CoreService.SavechanesAsnc();

            return RedirectToPage("/index");

        }
    }
}
