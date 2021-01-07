using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VetCoin.Migrations
{
    public partial class migration20733 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Venders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VetMemberId = table.Column<int>(type: "int", nullable: false),
                    HasDeliveryMessage = table.Column<bool>(type: "bit", nullable: false),
                    DeliveryMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdateUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsClosed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Venders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Venders_VetMembers_VetMemberId",
                        column: x => x.VetMemberId,
                        principalTable: "VetMembers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VenderLikeVotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VenderId = table.Column<int>(type: "int", nullable: false),
                    VetMemberId = table.Column<int>(type: "int", nullable: false),
                    CreateUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VenderLikeVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VenderLikeVotes_Venders_VenderId",
                        column: x => x.VenderId,
                        principalTable: "Venders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VenderLikeVotes_VetMembers_VetMemberId",
                        column: x => x.VetMemberId,
                        principalTable: "VetMembers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VenderMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VetMemberId = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VenderId = table.Column<int>(type: "int", nullable: false),
                    CreateUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VenderMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VenderMessages_Venders_VenderId",
                        column: x => x.VenderId,
                        principalTable: "Venders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VenderMessages_VetMembers_VetMemberId",
                        column: x => x.VetMemberId,
                        principalTable: "VetMembers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VenderSales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VenderId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VetMemberId = table.Column<int>(type: "int", nullable: false),
                    CreateUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VenderSales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VenderSales_Venders_VenderId",
                        column: x => x.VenderId,
                        principalTable: "Venders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VenderSales_VetMembers_VetMemberId",
                        column: x => x.VetMemberId,
                        principalTable: "VetMembers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_VenderLikeVotes_VenderId",
                table: "VenderLikeVotes",
                column: "VenderId");

            migrationBuilder.CreateIndex(
                name: "IX_VenderLikeVotes_VetMemberId",
                table: "VenderLikeVotes",
                column: "VetMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_VenderMessages_VenderId",
                table: "VenderMessages",
                column: "VenderId");

            migrationBuilder.CreateIndex(
                name: "IX_VenderMessages_VetMemberId",
                table: "VenderMessages",
                column: "VetMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Venders_VetMemberId",
                table: "Venders",
                column: "VetMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_VenderSales_VenderId",
                table: "VenderSales",
                column: "VenderId");

            migrationBuilder.CreateIndex(
                name: "IX_VenderSales_VetMemberId",
                table: "VenderSales",
                column: "VetMemberId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VenderLikeVotes");

            migrationBuilder.DropTable(
                name: "VenderMessages");

            migrationBuilder.DropTable(
                name: "VenderSales");

            migrationBuilder.DropTable(
                name: "Venders");
        }
    }
}
