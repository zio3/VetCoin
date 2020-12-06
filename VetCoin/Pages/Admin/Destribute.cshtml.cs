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

namespace VetCoin.Pages.Admin
{
    public class DestributeModel : PageModel
    {
        public DestributeModel(CoreService coreService, ApplicationDbContext dbContext)
        {
            CoreService = coreService;
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
        public bool IsIssued { get; set; }

        public long IssuedAmound { get; set; }

        public VetMember ReciveMember { get; set; }

        public long ReciverAmount { get; set; }

        public IActionResult OnGet(int? reciveVetMemberId)
        {
            UserContext = CoreService.GetUserContext();
            if (UserContext == null)
            {
                return NotFound();
            }
            MembersDdlInit();

            if (reciveVetMemberId.HasValue)
            {
                ReciveVetMemberId = reciveVetMemberId.Value;
            }

            return Page();
        }

        private void MembersDdlInit()
        {
            MembersDdl = DbContext.VetMembers
                .AsQueryable()
                .Where(c => c.MemberType == MemberType.User)
                 .OrderBy(c => c.Name)
                 .Select(c => new SelectListItem { Text = $"{c.Name}", Value = c.Id.ToString() });
        }

        public async Task<IActionResult> OnPostAsync(long sendAmount,string message ,int reciveVetMemberId)
        {
            UserContext = CoreService.GetUserContext();
            if (UserContext == null)
            {
                return NotFound();
            }

            //if (sendAmount <= 0)
            //{
            //    this.ModelState.AddModelError("sendAmount", "1以上の数を指定してください");
            //    return OnGet(null);
            //}
            //if (UserContext.Amount  < sendAmount)
            //{
            //    this.ModelState.AddModelError("sendAmount", "残高が不足しています");
            //    return OnGet(null);
            //}

            //CoreService.AddTransaction(UserContext , sendAmount, message, reciveVetMemberId, CoinTransactionType.Transfer);
            //await CoreService.SavechanesAsnc();

            var issure = DbContext.VetMembers.FirstOrDefault(c => c.MemberType == MemberType.Issuer);
            DbContext.CoinTransactions.Add(new CoinTransaction
            {
                Amount = sendAmount,
                SendeVetMemberId = issure.Id,
                RecivedVetMemberId = reciveVetMemberId,
                Text = message
            });

            DbContext.SaveChanges();

            MembersDdlInit();

            ReciveMember = DbContext.VetMembers.Find(reciveVetMemberId);
            IssuedAmound = sendAmount;

            IsIssued = true;

            ReciverAmount = CoreService.CalcAmount(ReciveMember);

            return Page();
        }
    }
}
