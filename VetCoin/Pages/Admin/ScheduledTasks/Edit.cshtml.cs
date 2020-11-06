using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VetCoin.Data;
using VetCoin.Data.JsonParamEntites;
using VetCoin.Services;
using VetCoin.Services.HostedServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace VetCoin.Pages.Admin.ScheduledTasks
{
    public class EditModel : PageModel
    {
        public EditModel(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public ApplicationDbContext DbContext { get; }

        [BindProperty]
        public ScheduleInfo ScheduleInfo { get; set; }

        public IActionResult OnGet(string methodName)
        {
            var mi = typeof(ScheduledExecutionService)
                .GetMethod(methodName);
            if (mi == null)
            {
                return NotFound();
            }

            if (mi.GetCustomAttributes(typeof(CronAttribute), true).Length == 0)
            {
                return NotFound();
            }

            var expression = (mi.GetCustomAttributes(typeof(CronAttribute), true)[0] as CronAttribute).CronExpression;

            ScheduleInfo = DbContext.GetParamArray<ScheduleInfo>().FirstOrDefault(c => c.MethodName == methodName);
            if (ScheduleInfo == null)
            {
                ScheduleInfo = new ScheduleInfo
                {
                    MethodName = methodName,
                };
            }

            ScheduleInfo.CronExpression = expression;

            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var infoes = DbContext.GetParamArray<ScheduleInfo>().ToList();

            var entity = infoes.FirstOrDefault(c => c.MethodName == ScheduleInfo.MethodName);
            if (entity != null)
            {
                entity.OverideCronExpression = ScheduleInfo.OverideCronExpression;
                entity.Disabled = ScheduleInfo.Disabled;
                entity.SendVerboseInfo = ScheduleInfo.SendVerboseInfo;
            }
            else
            {
                infoes.Add(ScheduleInfo);
            }

            DbContext.SetParamArray(infoes.ToArray());

            ScheduledExecutionHostedService<ScheduledExecutionService>.ScheduleInfoes = infoes.ToArray();

            try
            {
                await DbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }



            return RedirectToPage("./Index");
        }

    }
}