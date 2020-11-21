using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VetCoin.Services;

namespace VetCoin.Pages.Admin
{
    public class RegularDistributeModel : PageModel
    {
        public RegularDistributeModel(CoreService coreService )
        {
            CoreService = coreService;
        }

        public CoreService CoreService { get; }

        public MemberDistributeAmount[] MemberDistributeAmounts { get; set; }


        public int TotalDistributeAmount { get; set; }

        public async Task OnGetAsync()
        {
            TotalDistributeAmount =  CoreService.TotalDistibutionAmount();
            MemberDistributeAmounts = await CoreService.CalcMemberDistributeAmount(TotalDistributeAmount);

            MemberDistributeAmounts = MemberDistributeAmounts
                                        .OrderByDescending(c => c.Amount)
                                        .ThenBy(c => c.VetMember.Name)
                                        .ToArray();

        }
    }
}
