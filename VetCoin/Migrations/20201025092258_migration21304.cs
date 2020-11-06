using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VetCoin.Migrations
{
    public partial class migration21304 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JsonParams",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    JsonBody = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JsonParams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScheduledExecutionLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FunctionName = table.Column<string>(nullable: true),
                    Start = table.Column<DateTimeOffset>(nullable: false),
                    Finished = table.Column<DateTimeOffset>(nullable: true),
                    HasException = table.Column<bool>(nullable: false),
                    ExceptionMessage = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledExecutionLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleExecutionTickets",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    FunctionName = table.Column<string>(nullable: true),
                    Enviroment = table.Column<string>(nullable: true),
                    DateTime = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleExecutionTickets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VetMembers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DiscordId = table.Column<decimal>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    AvatarId = table.Column<string>(nullable: true),
                    MemberType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VetMembers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CoinTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SendeVetMemberId = table.Column<int>(nullable: false),
                    RecivedVetMemberId = table.Column<int>(nullable: false),
                    Amount = table.Column<long>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    CreateUser = table.Column<string>(nullable: true),
                    UpdateUser = table.Column<string>(nullable: true),
                    TransactionType = table.Column<int>(nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(nullable: false),
                    UpdateDate = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoinTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoinTransactions_VetMembers_RecivedVetMemberId",
                        column: x => x.RecivedVetMemberId,
                        principalTable: "VetMembers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CoinTransactions_VetMembers_SendeVetMemberId",
                        column: x => x.SendeVetMemberId,
                        principalTable: "VetMembers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Trades",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VetMemberId = table.Column<int>(nullable: false),
                    Direction = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    Content = table.Column<string>(nullable: false),
                    Reward = table.Column<int>(nullable: true),
                    RewardComment = table.Column<string>(nullable: true),
                    DeliveryDate = table.Column<string>(nullable: true),
                    TradeStatus = table.Column<int>(nullable: false),
                    ConsultationRequest = table.Column<int>(nullable: false),
                    CreateUser = table.Column<string>(nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(nullable: false),
                    UpdateUser = table.Column<string>(nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trades_VetMembers_VetMemberId",
                        column: x => x.VetMemberId,
                        principalTable: "VetMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contracts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TradeId = table.Column<int>(nullable: false),
                    Reword = table.Column<int>(nullable: false),
                    DeliveryDate = table.Column<string>(nullable: true),
                    AgreementContent = table.Column<string>(nullable: true),
                    TraderSigne = table.Column<bool>(nullable: false),
                    ContractorSigne = table.Column<bool>(nullable: false),
                    ConsultationRequest = table.Column<int>(nullable: false),
                    VetMemberId = table.Column<int>(nullable: false),
                    EscrowTransactionId = table.Column<int>(nullable: true),
                    ContractStatus = table.Column<int>(nullable: false),
                    CreateUser = table.Column<string>(nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(nullable: false),
                    UpdateUser = table.Column<string>(nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contracts_CoinTransactions_EscrowTransactionId",
                        column: x => x.EscrowTransactionId,
                        principalTable: "CoinTransactions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Contracts_Trades_TradeId",
                        column: x => x.TradeId,
                        principalTable: "Trades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Contracts_VetMembers_VetMemberId",
                        column: x => x.VetMemberId,
                        principalTable: "VetMembers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TradeMessages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VetMemberId = table.Column<int>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    TradeId = table.Column<int>(nullable: false),
                    CreateUser = table.Column<string>(nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TradeMessages_Trades_TradeId",
                        column: x => x.TradeId,
                        principalTable: "Trades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TradeMessages_VetMembers_VetMemberId",
                        column: x => x.VetMemberId,
                        principalTable: "VetMembers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ContractMessage",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VetMemberId = table.Column<int>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    ContractId = table.Column<int>(nullable: false),
                    CreateUser = table.Column<string>(nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractMessage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractMessage_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContractMessage_VetMembers_VetMemberId",
                        column: x => x.VetMemberId,
                        principalTable: "VetMembers",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "VetMembers",
                columns: new[] { "Id", "AvatarId", "DiscordId", "MemberType", "Name" },
                values: new object[] { 1, null, 0m, 0, "[System]" });

            migrationBuilder.CreateIndex(
                name: "IX_CoinTransactions_RecivedVetMemberId",
                table: "CoinTransactions",
                column: "RecivedVetMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_CoinTransactions_SendeVetMemberId",
                table: "CoinTransactions",
                column: "SendeVetMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractMessage_ContractId",
                table: "ContractMessage",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractMessage_VetMemberId",
                table: "ContractMessage",
                column: "VetMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_EscrowTransactionId",
                table: "Contracts",
                column: "EscrowTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_TradeId",
                table: "Contracts",
                column: "TradeId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_VetMemberId",
                table: "Contracts",
                column: "VetMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_TradeMessages_TradeId",
                table: "TradeMessages",
                column: "TradeId");

            migrationBuilder.CreateIndex(
                name: "IX_TradeMessages_VetMemberId",
                table: "TradeMessages",
                column: "VetMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Trades_VetMemberId",
                table: "Trades",
                column: "VetMemberId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContractMessage");

            migrationBuilder.DropTable(
                name: "JsonParams");

            migrationBuilder.DropTable(
                name: "ScheduledExecutionLogs");

            migrationBuilder.DropTable(
                name: "ScheduleExecutionTickets");

            migrationBuilder.DropTable(
                name: "TradeMessages");

            migrationBuilder.DropTable(
                name: "Contracts");

            migrationBuilder.DropTable(
                name: "CoinTransactions");

            migrationBuilder.DropTable(
                name: "Trades");

            migrationBuilder.DropTable(
                name: "VetMembers");
        }
    }
}
