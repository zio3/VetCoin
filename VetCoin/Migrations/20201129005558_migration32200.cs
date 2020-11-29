using Microsoft.EntityFrameworkCore.Migrations;

namespace VetCoin.Migrations
{
    public partial class migration32200 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CoinTransactionId",
                table: "Doners",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Doners_CoinTransactionId",
                table: "Doners",
                column: "CoinTransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Doners_CoinTransactions_CoinTransactionId",
                table: "Doners",
                column: "CoinTransactionId",
                principalTable: "CoinTransactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doners_CoinTransactions_CoinTransactionId",
                table: "Doners");

            migrationBuilder.DropIndex(
                name: "IX_Doners_CoinTransactionId",
                table: "Doners");

            migrationBuilder.DropColumn(
                name: "CoinTransactionId",
                table: "Doners");
        }
    }
}
