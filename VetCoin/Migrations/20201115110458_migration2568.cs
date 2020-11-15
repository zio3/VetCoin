using Microsoft.EntityFrameworkCore.Migrations;

namespace VetCoin.Migrations
{
    public partial class migration2568 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_VetMembers_DiscordId",
                table: "VetMembers",
                column: "DiscordId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VetMembers_DiscordId",
                table: "VetMembers");
        }
    }
}
