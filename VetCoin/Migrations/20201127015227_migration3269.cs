using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VetCoin.Migrations
{
    public partial class migration3269 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DonationMessage",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VetMemberId = table.Column<int>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    DonationId = table.Column<int>(nullable: false),
                    CreateUser = table.Column<string>(nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonationMessage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DonationMessage_Donations_DonationId",
                        column: x => x.DonationId,
                        principalTable: "Donations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DonationMessage_VetMembers_VetMemberId",
                        column: x => x.VetMemberId,
                        principalTable: "VetMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DonationMessage_DonationId",
                table: "DonationMessage",
                column: "DonationId");

            migrationBuilder.CreateIndex(
                name: "IX_DonationMessage_VetMemberId",
                table: "DonationMessage",
                column: "VetMemberId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DonationMessage");
        }
    }
}
