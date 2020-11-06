using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VetCoin.Data;
using VetCoin.Services.HostedServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace VetCoin.Pages.Admin.ScheduledTasks
{
    public class ScheduleSummaryModel : PageModel
    {
        public ScheduleSummaryModel(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public ApplicationDbContext DbContext { get; }

        public ScheduleSummaryRow[] Summaryes { get; set; }

        public void OnGet()
        {
            var miList = typeof(Services.ScheduledExecutionService).GetMethods();

            var summary = miList.SelectMany(c =>
            {
                var attributes = c.GetCustomAttributes(typeof(CronAttribute), false).OfType<CronAttribute>();
                return attributes.Select(d => new ScheduleSummaryRow
                {
                    FunctionName = c.Name,
                    CronExpression = d.CronExpression,
                });
            }).ToArray();

            var functionNames = summary.Select(c => c.FunctionName).Distinct();
            var lastExecuteInfoes = DbContext.ScheduleExecutionTickets
                .AsQueryable()
                .GroupBy(c => c.FunctionName)
                .Select(c => new
                {
                    FunctionName = c.Key,
                    ExecuteDate = c.Max(d => d.DateTime)
                }).ToArray();

            var lastErrorInfoes = DbContext.ScheduledExecutionLogs
                .AsQueryable()
                .Where(c => c.HasException)
                .GroupBy(c => c.FunctionName)
                .Select(c => new
                {
                    FunctionName = c.Key,
                    ExecuteDate = c.Max(d => d.Start)
                }).ToArray();

            foreach (var item in summary)
            {
                var lastExecute = lastExecuteInfoes.FirstOrDefault(c => c.FunctionName == item.FunctionName);
                var lastError = lastErrorInfoes.FirstOrDefault(c => c.FunctionName == item.FunctionName);

                if (lastExecute != null)
                {
                    item.LastExecuteDateTime = lastExecute.ExecuteDate;
                }

                if (lastError != null)
                {
                    item.LastErrorDateTime = lastError.ExecuteDate;
                }
                item.NexeExecuteDateTime = ScheduledExecutionHostedService<Services.ScheduledExecutionService>.GetNextTime(DateTimeOffset.Now, item.CronExpression);

                if (item.LastExecuteDateTime != null)
                {
                    item.ExecuteDiff = item.NexeExecuteDateTime - item.LastExecuteDateTime.Value;
                }
            }

            Summaryes = summary.ToArray();
        }

        public class ScheduleSummaryRow
        {
            public string FunctionName { get; set; }

            public string CronExpression { get; set; }

            public DateTimeOffset? LastExecuteDateTime { get; set; }

            public DateTimeOffset? LastErrorDateTime { get; set; }

            public DateTimeOffset NexeExecuteDateTime { get; set; }


            public TimeSpan ExecuteDiff { get; set; }

        }

    }
}