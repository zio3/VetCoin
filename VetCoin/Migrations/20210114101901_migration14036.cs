using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VetCoin.Migrations
{
    public partial class migration14036 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExteralApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CallbackUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VetMemberId = table.Column<int>(type: "int", nullable: false),
                    IsNotification = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExteralApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExteralApplications_VetMembers_VetMemberId",
                        column: x => x.VetMemberId,
                        principalTable: "VetMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExteralApplicationPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExteralApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiscordId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    IsPayd = table.Column<bool>(type: "bit", nullable: false),
                    RefJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpirationDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreateUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdateUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExteralApplicationPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExteralApplicationPayments_ExteralApplications_ExteralApplicationId",
                        column: x => x.ExteralApplicationId,
                        principalTable: "ExteralApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExteralApplicationPayments_ExteralApplicationId",
                table: "ExteralApplicationPayments",
                column: "ExteralApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_ExteralApplications_VetMemberId",
                table: "ExteralApplications",
                column: "VetMemberId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExteralApplicationPayments");

            migrationBuilder.DropTable(
                name: "ExteralApplications");
        }
    }
}
