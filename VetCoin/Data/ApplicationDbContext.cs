using VetCoin.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using VetCoin.Codes;
using System.Threading;

namespace VetCoin.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options

            , IHttpContextAccessor httpContextAccessor

            )
            : base(options)
        {
            HttpContextAccessor = httpContextAccessor;
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<CoinTransaction>()
                .HasOne(c => c.SendVetMember)
                .WithMany(c => c.SendTransactions)
                .HasForeignKey(c => c.SendeVetMemberId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<CoinTransaction>()
                .HasOne(c => c.RecivedVetMember)
                .WithMany(c => c.RecivedTransactions)
                .HasForeignKey(c => c.RecivedVetMemberId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Contract>()
                .HasOne(c=>c.EscrowTransaction)
                .WithMany(c=>c.EscrowContracts)
                .HasForeignKey(c=>c.EscrowTransactionId)
                .OnDelete(DeleteBehavior.NoAction);


            builder.Entity<Contract>()
                .HasOne(c => c.VetMember)
                .WithMany()
                .HasForeignKey(c => c.VetMemberId)
                .OnDelete(DeleteBehavior.NoAction);
            
            builder.Entity<TradeMessage>()
                .HasOne(c => c.VetMember)
                .WithMany()
                .HasForeignKey(c => c.VetMemberId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ContractMessage>()
                .HasOne(c => c.VetMember)
                .WithMany()
                .HasForeignKey(c => c.VetMemberId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<SubscriptionMember>()
                    .HasKey(t => new { t.VetMemberId, t.SubscriptionId });

            builder.Entity<SubscriptionMember>()
                .HasOne(c => c.VetMember)
                .WithMany()
                .HasForeignKey(c => c.VetMemberId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<SubscriptionMember>()
                .HasOne(c => c.Subscription)
                .WithMany(c=>c.SubscriptionMembers)
                .HasForeignKey(c => c.SubscriptionId)
                .OnDelete(DeleteBehavior.NoAction);


            builder.Entity<TradeLikeVote>()
                .HasOne(c => c.Trade)
                .WithMany(c=>c.TradeLikeVotes)
                .HasForeignKey(c => c.TradeId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<TradeLikeVote>()
                .HasOne(c => c.VetMember)
                .WithMany()
                .HasForeignKey(c => c.VetMemberId)
                .OnDelete(DeleteBehavior.NoAction);



            builder.Entity<DonationLikeVote>()
                .HasOne(c => c.Donation)
                .WithMany(c => c.DonationLikeVotes)
                .HasForeignKey(c => c.DonationId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<DonationLikeVote>()
                .HasOne(c => c.VetMember)
                .WithMany()
                .HasForeignKey(c => c.VetMemberId)
                .OnDelete(DeleteBehavior.NoAction);



            builder.Entity<VetMember>()
                .HasIndex(b => b.DiscordId);


            builder.Entity<Donation>()
                .HasOne(c => c.VetMember)
                .WithMany()
                .HasForeignKey(c => c.VetMemberId)
                .OnDelete(DeleteBehavior.NoAction);
            
            builder.Entity<Doner>()
                .HasOne(c => c.VetMember)
                .WithMany()
                .HasForeignKey(c => c.VetMemberId)
                .OnDelete(DeleteBehavior.NoAction);

        }

        public DbSet<ScheduledExecutionLog> ScheduledExecutionLogs { get; set; }
        public DbSet<ScheduleExecutionTicket> ScheduleExecutionTickets { get; set; }

        public IHttpContextAccessor HttpContextAccessor { get; }

        public DbSet<JsonParam> JsonParams { get; set; }

        public DbSet<VetMember> VetMembers { get; set; }
        public DbSet<CoinTransaction> CoinTransactions { get; set; }

        public DbSet<Trade> Trades { get; set; }
        public DbSet<TradeMessage> TradeMessages { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ContractMessage> ContractMessage { get; set; }

        public DbSet<TradeImage> TradeImages { get; set; }
        public DbSet<ContractImage> ContractImages { get; set; }

        public DbSet<Subscription> Subscriptions { get; set; }

        public DbSet<SubscriptionMember> SubscriptionMembers { get; set; }
        public DbSet<TradeLikeVote> TradeLikeVotes { get; set; }

        public DbSet<ReactionMap> ReactionMaps { get; set; }


        public DbSet<Donation> Donations { get; set; }
        public DbSet<DonationLog> DonationLogs { get; set; }
        public DbSet<Doner> Doners { get; set; }
        public DbSet<RuleTextLog> RuleTextLogs { get; set; }

        public DbSet<DonationLikeVote> DonationLikeVotes { get; set; }

        

        public DbSet<DonationMessage> DonationMessages { get; set; }

        public override int SaveChanges()
        {
            EntryModifyInfo();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            EntryModifyInfo();
            return await base.SaveChangesAsync(cancellationToken);
        }



        public T GetParam<T>() where T : new()
        {
            var typeName = typeof(T).Name;
            var entity = JsonParams.Find(typeName);
            if (entity == null)
            {
                return new T();
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(entity.JsonBody);
            }
        }

        public T[] GetParamArray<T>() where T : new()
        {
            var typeName = typeof(T).Name;
            var entity = JsonParams.Find(typeName + "[]");
            if (entity == null)
            {
                return new T[0];
            }
            else
            {
                return JsonConvert.DeserializeObject<T[]>(entity.JsonBody);
            }
        }


        public void SetParam<T>(T arg) where T : new()
        {
            var typeName = typeof(T).Name;
            var entity = JsonParams.Find(typeName);
            if (entity == null)
            {
                entity = new JsonParam
                {
                    Id = typeName
                };
                JsonParams.Add(entity);
            }
            entity.JsonBody = JsonConvert.SerializeObject(arg);
        }

        public void SetParamArray<T>(T[] args) where T : new()
        {
            var typeName = typeof(T).Name + "[]";
            var entity = JsonParams.Find(typeName);
            if (entity == null)
            {
                entity = new JsonParam
                {
                    Id = typeName
                };
                JsonParams.Add(entity);
            }
            entity.JsonBody = JsonConvert.SerializeObject(args);
        }

        private void EntryModifyInfo()
        {
            var saveChangesDate = DateTimeOffset.Now.ToOffset(Consts.JstOffset);
            var user = "[System]";

            if (HttpContextAccessor != null && HttpContextAccessor.HttpContext != null)
            {
                if (HttpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                {
                    user = HttpContextAccessor.HttpContext.User.Identity.Name;
                }
                else
                {
                    user = "[AnonymousWebRequest]";
                }
            }


            var entiteys = ChangeTracker.Entries();
            var createdEntityes = entiteys
                            .Where(c => c.State == EntityState.Added)
                            .Select(c => c.Entity)
                            .OfType<ICreate>()
                            .ToArray();
            foreach (var item in createdEntityes)
            {
                item.CreateDate = saveChangesDate;
                item.CreateUser = user;
            }
            var updateEntityes = entiteys
                 .Where(c => c.State == EntityState.Added || c.State == EntityState.Modified)
                 .Select(c => c.Entity)
                 .OfType<IUpdate>()
                 .ToArray();
            foreach (var item in updateEntityes)
            {
                item.UpdateDate = saveChangesDate;
                item.UpdateUser = user;
            }


            //var loggingEntities = entiteys
            //     .Where(c => c.State == EntityState.Added || c.State == EntityState.Modified || c.State == EntityState.Deleted)
            //     .Where(c => c.Entity is ILogging)
            //     .ToArray();

            //foreach (var item in loggingEntities)
            //{
            //    var modifyEntity = item.Entity;
            //    var orgId = (item.Entity as ILogging).GetOrigenKey();
            //    var modifyEntityJson = JsonConvert.SerializeObject(item.Entity);

            //    this.TableChangeHistories.Add(new TableChangeHistory
            //    {
            //        Body = modifyEntityJson,
            //        CreateDate = saveChangesDate,
            //        CreateUser = user,
            //        Type = item.Entity.GetType().FullName,
            //        EntityState = item.State,
            //        OrigenKey = orgId

            //    });
            //}


        }

    }
}




