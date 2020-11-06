using Microsoft.AspNetCore.Mvc.RazorPages;
using VetCoin.Data;
using VetCoin.Data.JsonParamEntites;
using VetCoin.Services;
using VetCoin.Services.HostedServices;
using System.Linq;

namespace VetCoin.Pages.Admin.ScheduledTasks
{
    public class IndexModel : PageModel
    {
        public IndexModel(ScheduledExecutionService scheduledExecutionService,
                        ApplicationDbContext dbContext
            )
        {
            ScheduledExecutionService = scheduledExecutionService;
            DbContext = dbContext;
        }

        public ScheduledExecutionService ScheduledExecutionService { get; }
        public ApplicationDbContext DbContext { get; }

        public ScheduleInfo[] ScheduleInfoes { get; set; }

        public void OnGet()
        {
            ScheduleInfoes = typeof(ScheduledExecutionService)
                .GetMethods()
                .Where(c => c.GetCustomAttributes(typeof(CronAttribute), true).Length != 0)
                .Select(c => new ScheduleInfo
                {
                    MethodName = c.Name,
                    CronExpression = (c.GetCustomAttributes(typeof(CronAttribute), true)[0] as CronAttribute).CronExpression
                }).ToArray();

            var dbScheduleInfoes = DbContext.GetParamArray<ScheduleInfo>();

            foreach (var scheduleInfo in ScheduleInfoes)
            {
                var dbEntity = dbScheduleInfoes.FirstOrDefault(c => c.MethodName == scheduleInfo.MethodName);
                if (dbEntity != null)
                {   //DBの内容で更新                    
                    scheduleInfo.OverideCronExpression = dbEntity.OverideCronExpression;
                    scheduleInfo.Disabled = dbEntity.Disabled;
                }
            }
        }
    }


}