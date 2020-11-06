using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VetCoin.Data;

namespace VetCoin.Services.HostedServices
{
    public class DbSeedHostedService : BackgroundService
    {


        public DbSeedHostedService(IServiceProvider services)
        {
            Services = services;
        }

        public IServiceProvider Services { get; }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = Services.CreateScope())
            {
                var dbContext = ActivatorUtilities.CreateInstance<ApplicationDbContext>(scope.ServiceProvider);

                var issuer = dbContext.VetMembers.FirstOrDefault(c => c.MemberType == MemberType.Issuer);
                var vault = dbContext.VetMembers.FirstOrDefault(c => c.MemberType == MemberType.Vault);
                var bank = dbContext.VetMembers.FirstOrDefault(c => c.MemberType == MemberType.Bank);
                var escrow = dbContext.VetMembers.FirstOrDefault(c => c.MemberType == MemberType.Escrow);
                
                if(issuer == null)
                {
                    dbContext.VetMembers.Add(new VetMember
                    {
                        Name = "通貨発行者",
                        MemberType = MemberType.Issuer                        
                    });
                }
                if (vault == null)
                {
                    dbContext.VetMembers.Add(new VetMember
                    {
                        Name = "通貨貯蔵庫",
                        MemberType = MemberType.Vault
                    });
                }
                if (bank == null)
                {
                    dbContext.VetMembers.Add(new VetMember
                    {
                        Name = "中央銀行",
                        MemberType = MemberType.Bank
                    });
                }
                if (escrow == null)
                {
                    dbContext.VetMembers.Add(new VetMember
                    {
                        Name = "エスクローユーザー",
                        MemberType = MemberType.Escrow
                    });
                }

                dbContext.SaveChanges();
            }

            return Task.CompletedTask;
        }
    }
}
