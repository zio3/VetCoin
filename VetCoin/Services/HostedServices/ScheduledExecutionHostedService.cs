using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using VetCoin.Services.Chat;
using VetCoin.Data.JsonParamEntites;
using VetCoin.Data;

namespace VetCoin.Services.HostedServices
{
    public class ScheduledExecutionHostedService<T> : IHostedService
    {
        public IServiceProvider Services { get; }
        public DiscordService DiscordService { get; }
        public ILogger<ScheduledExecutionHostedService<T>> Logger { get; }

        static public ScheduleInfo[] ScheduleInfoes { get; set; }

        public ScheduledExecutionHostedService(IServiceProvider services,
            //   IOptions<AppSettings> options,
            DiscordService discordService,
            ILogger<ScheduledExecutionHostedService<T>> logger
            )
        {
            Services = services;
            DiscordService = discordService;
            Logger = logger;
            //   AppSettings = options.Value;
        }

        List<Task> ScheduleTasks = new List<Task>();

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var miList = typeof(T).GetMethods();

            using (var scope = Services.CreateScope())
            {
                var dbContext = ActivatorUtilities.CreateInstance<ApplicationDbContext>(scope.ServiceProvider);
                ScheduleInfoes = dbContext.GetParamArray<ScheduleInfo>();
            }

            //if (AppSettings.EnableSchedule)
            {
                foreach (var mi in miList)
                {
                    var cronAttributes = mi.GetCustomAttributes(typeof(CronAttribute), false).Cast<CronAttribute>();
                    foreach (var cronAttribute in cronAttributes)
                    {
                        ScheduleTasks.Add(CronTaskLoop(mi, cronAttribute.CronExpression));
                    }
                }
            }
            return Task.CompletedTask;
        }

        private ScheduleInfo GetScheduleInfo(string methodName)
        {
            var entity = ScheduleInfoes.FirstOrDefault(c => c.MethodName == methodName) ?? new ScheduleInfo();
            return entity;
        }

        async Task CronTaskLoop(MethodInfo mi, string cronExpression)
        {
            try
            {

                while (true)
                {
                    Logger.LogWarning($"LoopStart {mi.Name}");

                    var scheduleInfo = GetScheduleInfo(mi.Name);

                    if (scheduleInfo.Disabled)
                    {
                        await Task.Delay(TimeSpan.FromMinutes(5));
                        continue;
                    }

                    var now = DateTimeOffset.Now;
                    var nextTime = await GetNextTimeAsync(now, cronExpression, scheduleInfo.OverideCronExpression);

                    if (nextTime == null)
                    {
                        await Task.Delay(TimeSpan.FromMinutes(5));
                        continue;
                    }


                    //await DiscordService.SendMessage(DiscordService.Channel.Verbose, $"Method:{mi.Name} Now:{now} Next:{nextTime} Diff{nextTime.Value - now}");
                    Logger.LogInformation($"Method:{mi.Name} Now:{now} Next:{nextTime} Diff{nextTime.Value - now}");
                    await Task.Delay(nextTime.Value - now);

                    var ticketKey = $"Service:{typeof(T).Name} Method:{mi.Name} Time:{nextTime.Value.ToString("yyyy/MM/dd HH:mm:ss")}";

                    scheduleInfo = GetScheduleInfo(mi.Name);
                    if (scheduleInfo.Disabled)
                    {
                        await Task.Delay(TimeSpan.FromMinutes(5));
                        continue;
                    }



                    using (var scope = Services.CreateScope())
                    {
                        var DbContext = ActivatorUtilities.CreateInstance<ApplicationDbContext>(scope.ServiceProvider);
                        DbContext.ScheduleExecutionTickets.Add(new ScheduleExecutionTicket
                        {
                            Id = ticketKey,
                            FunctionName = mi.Name,
                            DateTime = DateTimeOffset.Now,
                            Enviroment = Environment.MachineName
                        });

                        try
                        {
                            await DbContext.SaveChangesAsync();
                        }
                        catch
                        {
                            //書き込めなかった場合、実行しない
                            continue;
                        }
                    }

                    Logger.LogWarning($"TaskStart {ticketKey}");
                    //await DiscordService.SendMessage(DiscordService.Channel.Verbose, $"Method:{mi.Name} Now:{now} TaskStart:{ticketKey}");


                    var t = Task.Run(async () =>
                    {
                        using (var scope = Services.CreateScope())
                        {
                            var DbContext = ActivatorUtilities.CreateInstance<ApplicationDbContext>(scope.ServiceProvider);
                            var scheduleService = scope.ServiceProvider.GetService<T>();
                            await InvokeTask(DbContext, mi, scheduleService);

                            Logger.LogWarning($"TaskEnd {ticketKey}");
                        }
                    });
                }
            }
            catch (TaskCanceledException)
            {

            }
            catch (Exception e)
            {
                await NotificationException(e);
            }
        }

        public async Task NotificationException(Exception e)
        {
            await DiscordService.SendMessage(DiscordService.Channel.ScheduleError, $"{e}");
            Logger.LogError(e, "VetCoin 例外2");
        }

        public async Task<InvokeResult> InvokeTask(ApplicationDbContext dbContext, MethodInfo mi, Object scheduleService)
        {
            var r = new InvokeResult();
            r.MethodName = mi.Name;
            r.Start = DateTimeOffset.Now;

            var logItem = new ScheduledExecutionLog();
            dbContext.ScheduledExecutionLogs.Add(logItem);

            try
            {
                logItem.Start = DateTimeOffset.Now;
                logItem.FunctionName = mi.Name;
                await dbContext.SaveChangesAsync();
                var invokeResult = mi.Invoke(scheduleService, null);
                if (invokeResult is Task)
                {
                    await (Task)invokeResult;
                }
            }
            catch (Exception e)
            {
                logItem.HasException = true;
                logItem.ExceptionMessage = e.ToString();
                r.Exception = e;
                await NotificationException(e);
            }
            logItem.Finished = DateTimeOffset.Now;
            r.Fineshed = DateTimeOffset.Now;
            await dbContext.SaveChangesAsync();

            return r;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task<DateTimeOffset?> GetNextTimeAsync(DateTimeOffset now, string expression, string overrideExpression)
        {
            if (!string.IsNullOrEmpty(overrideExpression))
            {
                try
                {
                    return GetNextTime(now, overrideExpression);
                }
                catch
                {
                    await DiscordService.SendMessage(DiscordService.Channel.ScheduleError, $"Cron式が解釈できません:{overrideExpression}");
                }
            }

            if (!string.IsNullOrEmpty(expression))
            {
                try
                {
                    return GetNextTime(now, expression);
                }
                catch
                {
                    await DiscordService.SendMessage(DiscordService.Channel.ScheduleError, $"Cron式が解釈できません:{expression}");
                }
            }

            return null;
        }

        public static DateTimeOffset GetNextTime(DateTimeOffset now, string expression)
        {
            var options = new NCrontab.CrontabSchedule.ParseOptions
            {
            };
            var crontabSchedule = NCrontab.CrontabSchedule.Parse(expression, options);
            var jstOffset = TimeSpan.FromHours(9);
            var jstNow = now.ToOffset(jstOffset).DateTime;
            var jstNextDate = crontabSchedule.GetNextOccurrence(jstNow);

            var unspecified = DateTime.SpecifyKind(jstNextDate, DateTimeKind.Unspecified);

            return new DateTimeOffset(unspecified, jstOffset);
        }

        public class InvokeResult
        {
            public string MethodName { get; set; }
            public Exception Exception { get; set; }
            public DateTimeOffset Start { get; set; }

            public DateTimeOffset Fineshed { get; set; }
        }


    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    class CronAttribute : Attribute
    {
        public string CronExpression { get; }

        public CronAttribute(string cronExpression)
        {
            CronExpression = cronExpression;
        }
    }
}
