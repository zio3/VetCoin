using Microsoft.EntityFrameworkCore.Migrations;

namespace VetCoin.Migrations
{
    public partial class migration22714 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubscriptionMembers_Subscriptions_SubscriptionId1",
                table: "SubscriptionMembers");

            migrationBuilder.DropIndex(
                name: "IX_SubscriptionMembers_SubscriptionId1",
                table: "SubscriptionMembers");

            migrationBuilder.DropColumn(
                name: "SubscriptionId1",
                table: "SubscriptionMembers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubscriptionId1",
                table: "SubscriptionMembers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionMembers_SubscriptionId1",
                table: "SubscriptionMembers",
                column: "SubscriptionId1");

            migrationBuilder.AddForeignKey(
                name: "FK_SubscriptionMembers_Subscriptions_SubscriptionId1",
                table: "SubscriptionMembers",
                column: "SubscriptionId1",
                principalTable: "Subscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
