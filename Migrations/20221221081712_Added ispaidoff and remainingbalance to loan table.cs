using Microsoft.EntityFrameworkCore.Migrations;

namespace ASHMONEY_API.Migrations
{
    public partial class Addedispaidoffandremainingbalancetoloantable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPaidOff",
                table: "Loans",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "RemainingBalance",
                table: "Loans",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPaidOff",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "RemainingBalance",
                table: "Loans");
        }
    }
}
