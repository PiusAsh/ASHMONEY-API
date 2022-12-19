using Microsoft.EntityFrameworkCore.Migrations;

namespace ASHMONEY_API.Migrations
{
    public partial class CHANGEDTRANSACTIONTABLE : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Transactions");

            migrationBuilder.AddColumn<string>(
                name: "BeneficiaryAccount",
                table: "Transactions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BeneficiaryBankName",
                table: "Transactions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SenderAccount",
                table: "Transactions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BeneficiaryAccount",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "BeneficiaryBankName",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "SenderAccount",
                table: "Transactions");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
