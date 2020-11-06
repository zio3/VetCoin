using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VetCoin.Data;

namespace VetCoin.Pages.Admin
{
    public class StartModel : PageModel
    {
        public StartModel(ApplicationDbContext applicationDbContext)
        {
            ApplicationDbContext = applicationDbContext;
        }

        public ApplicationDbContext ApplicationDbContext { get; }

        public bool IsDistributed { get; set; }

        public IActionResult OnGet()
        {
            //if(User.Identity.Name != "zio3")
            //{
            //    return NotFound();
            //}

            if( ApplicationDbContext.CoinTransactions.Count() != 0)
            {
                IsDistributed = true;
            }

            return Page();
        }

        public void OnPost()
        {
            if (ApplicationDbContext.CoinTransactions.Count() != 0)
            {
                IsDistributed = true;
            }


            if (IsDistributed == false)
            {
                var dbContext = ApplicationDbContext;

                var issuer = dbContext.VetMembers.FirstOrDefault(c => c.MemberType == MemberType.Issuer);
                var vault = dbContext.VetMembers.FirstOrDefault(c => c.MemberType == MemberType.Vault);
                var bank = dbContext.VetMembers.FirstOrDefault(c => c.MemberType == MemberType.Bank);
                var escrow = dbContext.VetMembers.FirstOrDefault(c => c.MemberType == MemberType.Escrow);

                var unRecivedUsers = dbContext.VetMembers
                    .AsQueryable()
                    .Where(c => c.MemberType == MemberType.User)
                    .Where(c => c.RecivedTransactions.Count() == 0)
                    .ToArray();

                foreach (var item in unRecivedUsers)
                {

                    var issueTran = new CoinTransaction
                    {
                        Amount = 100000,
                        SendVetMember = issuer,
                        RecivedVetMember = vault,
                        TransactionType = CoinTransactionType.Issue,
                        Text = $"[登録時発行:{item.Name}]"
                    };

                    var initTran = new CoinTransaction
                    {
                        Amount = 4000,
                        SendVetMember = vault,
                        RecivedVetMemberId = item.Id,
                        Text = "[開始時配布コイン]",
                        TransactionType = CoinTransactionType.InitialDistribution,
                    };

                    dbContext.CoinTransactions.Add(issueTran);
                    dbContext.CoinTransactions.Add(initTran);
                }

                ApplicationDbContext.SaveChanges();
                IsDistributed = true;
            }
        }
    }
}
