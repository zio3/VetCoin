using Microsoft.EntityFrameworkCore.Migrations;

namespace VetCoin.Migrations
{
    public partial class migration1414 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DonationMessage_Donations_DonationId",
                table: "DonationMessage");

            migrationBuilder.DropForeignKey(
                name: "FK_DonationMessage_VetMembers_VetMemberId",
                table: "DonationMessage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DonationMessage",
                table: "DonationMessage");

            migrationBuilder.RenameTable(
                name: "DonationMessage",
                newName: "DonationMessages");

            migrationBuilder.RenameIndex(
                name: "IX_DonationMessage_VetMemberId",
                table: "DonationMessages",
                newName: "IX_DonationMessages_VetMemberId");

            migrationBuilder.RenameIndex(
                name: "IX_DonationMessage_DonationId",
                table: "DonationMessages",
                newName: "IX_DonationMessages_DonationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DonationMessages",
                table: "DonationMessages",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DonationMessages_Donations_DonationId",
                table: "DonationMessages",
                column: "DonationId",
                principalTable: "Donations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DonationMessages_VetMembers_VetMemberId",
                table: "DonationMessages",
                column: "VetMemberId",
                principalTable: "VetMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DonationMessages_Donations_DonationId",
                table: "DonationMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_DonationMessages_VetMembers_VetMemberId",
                table: "DonationMessages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DonationMessages",
                table: "DonationMessages");

            migrationBuilder.RenameTable(
                name: "DonationMessages",
                newName: "DonationMessage");

            migrationBuilder.RenameIndex(
                name: "IX_DonationMessages_VetMemberId",
                table: "DonationMessage",
                newName: "IX_DonationMessage_VetMemberId");

            migrationBuilder.RenameIndex(
                name: "IX_DonationMessages_DonationId",
                table: "DonationMessage",
                newName: "IX_DonationMessage_DonationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DonationMessage",
                table: "DonationMessage",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DonationMessage_Donations_DonationId",
                table: "DonationMessage",
                column: "DonationId",
                principalTable: "Donations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DonationMessage_VetMembers_VetMemberId",
                table: "DonationMessage",
                column: "VetMemberId",
                principalTable: "VetMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
