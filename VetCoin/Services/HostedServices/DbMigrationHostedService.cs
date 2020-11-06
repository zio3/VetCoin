using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VetCoin.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace VetCoin.Services.HostedServices
{
    public class DbMigrationHostedService<T> : BackgroundService
        where T : DbContext
    {


        public DbMigrationHostedService(IServiceProvider services)
        {
            Services = services;
        }

        public IServiceProvider Services { get; }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = Services.CreateScope())
            {
                var context = ActivatorUtilities.CreateInstance<T>(scope.ServiceProvider);
                // 自動マイグレーション適用
                context.Database.Migrate();
            }
            return Task.CompletedTask;
        }
    }
}
