using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VetCoin.Migrations
{
    public partial class migration17589 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(nullable: true),
                    Content = table.Column<string>(nullable: true),
                    Fee = table.Column<int>(nullable: false),
                    CreateUser = table.Column<string>(nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(nullable: false),
                    UpdateUser = table.Column<string>(nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionMembers",
                columns: table => new
                {
                    SubscriptionId = table.Column<int>(nullable: false),
                    VetMemberId = table.Column<int>(nullable: false),
                    CreateUser = table.Column<string>(nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(nullable: false),
                    SubscriptionId1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionMembers", x => new { x.VetMemberId, x.SubscriptionId });
                    table.ForeignKey(
                        name: "FK_SubscriptionMembers_Subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "Subscriptions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SubscriptionMembers_Subscriptions_SubscriptionId1",
                        column: x => x.SubscriptionId1,
                        principalTable: "Subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubscriptionMembers_VetMembers_VetMemberId",
                        column: x => x.VetMemberId,
                        principalTable: "VetMembers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionMembers_SubscriptionId",
                table: "SubscriptionMembers",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionMembers_SubscriptionId1",
                table: "SubscriptionMembers",
                column: "SubscriptionId1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubscriptionMembers");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.InsertData(
                table: "VetMembers",
                columns: new[] { "Id", "AvatarId", "DiscordId", "MemberType", "Name" },
                values: new object[] { 1, null, 0m, 0, "[System]" });
        }
    }
}
