using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VetCoin.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace VetCoin.Pages.Admin.ScheduledTasks
{
    public class HistoryModel : PageModel
    {
        public HistoryModel(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public ApplicationDbContext DbContext { get; }

        public IQueryable<ScheduledExecutionLog> Logs { get; set; }

        public bool IsErrorOnly { get; set; }

        public string MethodName { get; set; }

        public IActionResult OnGet(string methodName, bool isErrorOnly)
        {
            Logs = DbContext
                .ScheduledExecutionLogs
                .AsQueryable()
                .Where(c => c.FunctionName == methodName)
                .OrderByDescending(c => c.Start)
                .AsQueryable();

            if (isErrorOnly)
            {
                Logs = Logs.Where(c => c.HasException);
            }

            MethodName = methodName;

            return Page();
        }
    }
}