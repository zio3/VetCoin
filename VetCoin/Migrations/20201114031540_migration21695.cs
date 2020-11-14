using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VetCoin.Migrations
{
    public partial class migration21695 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TradeLikeVotes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TradeId = table.Column<int>(nullable: false),
                    VetMemberId = table.Column<int>(nullable: false),
                    CreateUser = table.Column<string>(nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeLikeVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TradeLikeVotes_Trades_TradeId",
                        column: x => x.TradeId,
                        principalTable: "Trades",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TradeLikeVotes_VetMembers_VetMemberId",
                        column: x => x.VetMemberId,
                        principalTable: "VetMembers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TradeLikeVotes_TradeId",
                table: "TradeLikeVotes",
                column: "TradeId");

            migrationBuilder.CreateIndex(
                name: "IX_TradeLikeVotes_VetMemberId",
                table: "TradeLikeVotes",
                column: "VetMemberId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TradeLikeVotes");
        }
    }
}
