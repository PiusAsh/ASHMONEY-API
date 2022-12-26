using Microsoft.EntityFrameworkCore.Migrations;

namespace ASHMONEY_API.Migrations
{
    public partial class AddedispaidoffandAmountPaidtoloantable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemainingBalance",
                table: "Loans");

            migrationBuilder.AddColumn<decimal>(
                name: "AmountPaid",
                table: "Loans",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountPaid",
                table: "Loans");

            migrationBuilder.AddColumn<decimal>(
                name: "RemainingBalance",
                table: "Loans",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
