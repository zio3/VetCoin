using Microsoft.EntityFrameworkCore.Migrations;

namespace VetCoin.Migrations
{
    public partial class migration971 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VetMemberId",
                table: "Subscriptions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_VetMemberId",
                table: "Subscriptions",
                column: "VetMemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_VetMembers_VetMemberId",
                table: "Subscriptions",
                column: "VetMemberId",
                principalTable: "VetMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_VetMembers_VetMemberId",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_VetMemberId",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "VetMemberId",
                table: "Subscriptions");
        }
    }
}
