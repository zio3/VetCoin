﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VetCoin.Data;

namespace VetCoin.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("VetCoin.Data.CoinTransaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("Amount")
                        .HasColumnType("bigint");

                    b.Property<DateTimeOffset>("CreateDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("CreateUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RecivedVetMemberId")
                        .HasColumnType("int");

                    b.Property<int>("SendeVetMemberId")
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TransactionType")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset>("UpdateDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("UpdateUser")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("RecivedVetMemberId");

                    b.HasIndex("SendeVetMemberId");

                    b.ToTable("CoinTransactions");
                });

            modelBuilder.Entity("VetCoin.Data.Contract", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AgreementContent")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ConsultationRequest")
                        .HasColumnType("int");

                    b.Property<int>("ContractStatus")
                        .HasColumnType("int");

                    b.Property<bool>("ContractorSigne")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset>("CreateDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("CreateUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DeliveryDate")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("EscrowTransactionId")
                        .HasColumnType("int");

                    b.Property<int>("Reword")
                        .HasColumnType("int");

                    b.Property<int>("TradeId")
                        .HasColumnType("int");

                    b.Property<bool>("TraderSigne")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset>("UpdateDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("UpdateUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("VetMemberId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("EscrowTransactionId");

                    b.HasIndex("TradeId");

                    b.HasIndex("VetMemberId");

                    b.ToTable("Contracts");
                });

            modelBuilder.Entity("VetCoin.Data.ContractImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ContractId")
                        .HasColumnType("int");

                    b.Property<byte[]>("ImageContent")
                        .HasColumnType("varbinary(max)");

                    b.HasKey("Id");

                    b.HasIndex("ContractId");

                    b.ToTable("ContractImages");
                });

            modelBuilder.Entity("VetCoin.Data.ContractMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ContractId")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset>("CreateDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("CreateUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Message")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("VetMemberId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ContractId");

                    b.HasIndex("VetMemberId");

                    b.ToTable("ContractMessage");
                });

            modelBuilder.Entity("VetCoin.Data.JsonParam", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("JsonBody")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("JsonParams");
                });

            modelBuilder.Entity("VetCoin.Data.ScheduleExecutionTicket", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTimeOffset>("DateTime")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Enviroment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FunctionName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ScheduleExecutionTickets");
                });

            modelBuilder.Entity("VetCoin.Data.ScheduledExecutionLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ExceptionMessage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset?>("Finished")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("FunctionName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("HasException")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset>("Start")
                        .HasColumnType("datetimeoffset");

                    b.HasKey("Id");

                    b.ToTable("ScheduledExecutionLogs");
                });

            modelBuilder.Entity("VetCoin.Data.Subscription", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset>("CreateDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("CreateUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Fee")
                        .HasColumnType("int");

                    b.Property<int>("SubscriptionStatus")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset>("UpdateDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("UpdateUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("VetMemberId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("VetMemberId");

                    b.ToTable("Subscriptions");
                });

            modelBuilder.Entity("VetCoin.Data.SubscriptionMember", b =>
                {
                    b.Property<int>("VetMemberId")
                        .HasColumnType("int");

                    b.Property<int>("SubscriptionId")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset>("CreateDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("CreateUser")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("VetMemberId", "SubscriptionId");

                    b.HasIndex("SubscriptionId");

                    b.ToTable("SubscriptionMembers");
                });

            modelBuilder.Entity("VetCoin.Data.Trade", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ConsultationRequest")
                        .HasColumnType("int");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset>("CreateDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("CreateUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DeliveryDate")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Direction")
                        .HasColumnType("int");

                    b.Property<bool>("IsContinued")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset>("OrderRefDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<int?>("Reward")
                        .HasColumnType("int");

                    b.Property<string>("RewardComment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TradeStatus")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset>("UpdateDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("UpdateUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("VetMemberId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("VetMemberId");

                    b.ToTable("Trades");
                });

            modelBuilder.Entity("VetCoin.Data.TradeImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<byte[]>("ImageContent")
                        .HasColumnType("varbinary(max)");

                    b.Property<int>("TradeId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TradeId");

                    b.ToTable("TradeImages");
                });

            modelBuilder.Entity("VetCoin.Data.TradeLikeVote", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTimeOffset>("CreateDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("CreateUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TradeId")
                        .HasColumnType("int");

                    b.Property<int>("VetMemberId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TradeId");

                    b.HasIndex("VetMemberId");

                    b.ToTable("TradeLikeVotes");
                });

            modelBuilder.Entity("VetCoin.Data.TradeMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTimeOffset>("CreateDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("CreateUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Message")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TradeId")
                        .HasColumnType("int");

                    b.Property<int>("VetMemberId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TradeId");

                    b.HasIndex("VetMemberId");

                    b.ToTable("TradeMessages");
                });

            modelBuilder.Entity("VetCoin.Data.VetMember", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AvatarId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("DiscordId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<int>("MemberType")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("DiscordId");

                    b.ToTable("VetMembers");
                });

            modelBuilder.Entity("VetCoin.Data.CoinTransaction", b =>
                {
                    b.HasOne("VetCoin.Data.VetMember", "RecivedVetMember")
                        .WithMany("RecivedTransactions")
                        .HasForeignKey("RecivedVetMemberId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("VetCoin.Data.VetMember", "SendVetMember")
                        .WithMany("SendTransactions")
                        .HasForeignKey("SendeVetMemberId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("VetCoin.Data.Contract", b =>
                {
                    b.HasOne("VetCoin.Data.CoinTransaction", "EscrowTransaction")
                        .WithMany("EscrowContracts")
                        .HasForeignKey("EscrowTransactionId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("VetCoin.Data.Trade", "Trade")
                        .WithMany("Contracts")
                        .HasForeignKey("TradeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VetCoin.Data.VetMember", "VetMember")
                        .WithMany()
                        .HasForeignKey("VetMemberId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("VetCoin.Data.ContractImage", b =>
                {
                    b.HasOne("VetCoin.Data.Contract", "Contract")
                        .WithMany("ContractImages")
                        .HasForeignKey("ContractId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VetCoin.Data.ContractMessage", b =>
                {
                    b.HasOne("VetCoin.Data.Contract", "Contract")
                        .WithMany("ContractMessages")
                        .HasForeignKey("ContractId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VetCoin.Data.VetMember", "VetMember")
                        .WithMany()
                        .HasForeignKey("VetMemberId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("VetCoin.Data.Subscription", b =>
                {
                    b.HasOne("VetCoin.Data.VetMember", "VetMember")
                        .WithMany()
                        .HasForeignKey("VetMemberId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VetCoin.Data.SubscriptionMember", b =>
                {
                    b.HasOne("VetCoin.Data.Subscription", "Subscription")
                        .WithMany("SubscriptionMembers")
                        .HasForeignKey("SubscriptionId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("VetCoin.Data.VetMember", "VetMember")
                        .WithMany()
                        .HasForeignKey("VetMemberId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("VetCoin.Data.Trade", b =>
                {
                    b.HasOne("VetCoin.Data.VetMember", "VetMember")
                        .WithMany()
                        .HasForeignKey("VetMemberId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VetCoin.Data.TradeImage", b =>
                {
                    b.HasOne("VetCoin.Data.Trade", "Trade")
                        .WithMany("TradeImages")
                        .HasForeignKey("TradeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VetCoin.Data.TradeLikeVote", b =>
                {
                    b.HasOne("VetCoin.Data.Trade", "Trade")
                        .WithMany("TradeLikeVotes")
                        .HasForeignKey("TradeId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("VetCoin.Data.VetMember", "VetMember")
                        .WithMany()
                        .HasForeignKey("VetMemberId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("VetCoin.Data.TradeMessage", b =>
                {
                    b.HasOne("VetCoin.Data.Trade", "Trade")
                        .WithMany("TradeMessages")
                        .HasForeignKey("TradeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VetCoin.Data.VetMember", "VetMember")
                        .WithMany()
                        .HasForeignKey("VetMemberId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
