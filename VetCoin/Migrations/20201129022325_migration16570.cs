using Microsoft.EntityFrameworkCore.Migrations;

namespace VetCoin.Migrations
{
    public partial class migration16570 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doners_CoinTransactions_CoinTransactionId",
                table: "Doners");

            migrationBuilder.AlterColumn<int>(
                name: "CoinTransactionId",
                table: "Doners",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Doners_CoinTransactions_CoinTransactionId",
                table: "Doners",
                column: "CoinTransactionId",
                principalTable: "CoinTransactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doners_CoinTransactions_CoinTransactionId",
                table: "Doners");

            migrationBuilder.AlterColumn<int>(
                name: "CoinTransactionId",
                table: "Doners",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Doners_CoinTransactions_CoinTransactionId",
                table: "Doners",
                column: "CoinTransactionId",
                principalTable: "CoinTransactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
