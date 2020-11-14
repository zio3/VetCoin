using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VetCoin.Data;
using VetCoin.Services;

namespace VetCoin.Pages
{
    public class AllLogsModel : PageModel
    {
        readonly Encoding sjisEnc = Encoding.GetEncoding("shift_jis");

        public AllLogsModel(ApplicationDbContext dbContext, CoreService coreService)
        {
            DbContext = dbContext;
            CoreService = coreService;
        }

        public ApplicationDbContext DbContext { get; }
        public CoreService CoreService { get; }
        public IQueryable<CoinTransaction> CoinTransactionQuery { get; set; }

        [BindProperty(SupportsGet =true)]
        public bool ExceptSamePersonTransaction { get; set; }

        public async Task<IActionResult> OnGet(string searchKey, string mode, bool exceptSamePersonTransaction )
        {
            switch (mode)
            {
                case "downloadCsv":
                    return await DownloadCsv(exceptSamePersonTransaction);
                default:
                    return await Search(searchKey);
            }

        }

        public async Task<IActionResult> DownloadCsv(bool exceptSamePersonTransaction)
        {
            await Task.Yield();
            var csv = CoreService.CsvExportAllTransactions(exceptSamePersonTransaction);
            return File(sjisEnc.GetBytes(csv), "text/csv", "VetCointTransactions.csv");
        }

        public async Task<IActionResult> Search(string searchKey)
        {
            await Task.Yield();
            CoinTransactionQuery = DbContext.CoinTransactions
                .Include(c => c.RecivedVetMember)
                .Include(c => c.SendVetMember)
                .OrderByDescending(c => c.UpdateDate)
                .AsQueryable();

            return Page();
        }
    }
}
