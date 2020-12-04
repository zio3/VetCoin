using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VetCoin.Migrations
{
    public partial class migration6392 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DonationLikeVotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DonationId = table.Column<int>(type: "int", nullable: false),
                    VetMemberId = table.Column<int>(type: "int", nullable: false),
                    CreateUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonationLikeVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DonationLikeVotes_Donations_DonationId",
                        column: x => x.DonationId,
                        principalTable: "Donations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DonationLikeVotes_VetMembers_VetMemberId",
                        column: x => x.VetMemberId,
                        principalTable: "VetMembers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DonationLikeVotes_DonationId",
                table: "DonationLikeVotes",
                column: "DonationId");

            migrationBuilder.CreateIndex(
                name: "IX_DonationLikeVotes_VetMemberId",
                table: "DonationLikeVotes",
                column: "VetMemberId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DonationLikeVotes");
        }
    }
}
