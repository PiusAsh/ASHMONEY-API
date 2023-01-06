using Microsoft.EntityFrameworkCore.Migrations;

namespace ASHMONEY_API.Migrations
{
    public partial class AddedLoanableAmountcolumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EligibleLoanAmt",
                table: "Accounts",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EligibleLoanAmt",
                table: "Accounts");
        }
    }
}
