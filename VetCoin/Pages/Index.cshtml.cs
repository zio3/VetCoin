using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VetCoin.Data;
using VetCoin.Services;

namespace VetCoin.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly CoreService CoreService;

        public UserContext UserContext { get; set; }

        public IQueryable<CoinTransaction> Transactions { get; set; }

        public string[] FileList { get; set; }
        public IWebHostEnvironment HostingEnvironment { get; }

        public IndexModel(ILogger<IndexModel> logger, CoreService coreService, IWebHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            CoreService = coreService;
            HostingEnvironment = hostingEnvironment;
        }

        public IActionResult OnGet()
        {
            try
            {

                var path = Path.Combine(HostingEnvironment.ContentRootPath, "Pages/");

                var uri = new Uri(path);
                var cshtmlPathList = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories);
                var targets = new[] { ".cs", ".cshtml" };
                cshtmlPathList = cshtmlPathList.Where(c => targets.Contains(Path.GetExtension(c).ToLower()));

                var fileList = cshtmlPathList
                    .Select(c => new {
                        Key = c.Replace(".cshtml.cs", string.Empty).Replace(".cshtml", string.Empty),
                        Date = new FileInfo(c).LastWriteTime,
                    })
                    .OrderByDescending(c => c.Date)
                    .Select(c => c.Key)
                    .Distinct()
                    .Select(c => uri.MakeRelativeUri(new Uri(c)).ToString())
                    .ToArray();
                FileList = fileList;

            }
            catch
            {

            }


            if (User.Identity.IsAuthenticated)
            {
                UserContext = CoreService.GetUserContext();
                if (UserContext == null)
                {
                    return RedirectToPage("SignOut");
                }

                Transactions = CoreService
                                    .GetCoinTransactionQuery(UserContext.CurrentUser)
                                    .Include(c=>c.RecivedVetMember)
                                    .Include(c => c.SendVetMember)
                                    .OrderByDescending(c => c.CreateDate)
                                    .Take(20);
            }

            return Page();
        }
    }
}
