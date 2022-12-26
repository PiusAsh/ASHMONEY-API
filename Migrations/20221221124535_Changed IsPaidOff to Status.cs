using Microsoft.EntityFrameworkCore.Migrations;

namespace ASHMONEY_API.Migrations
{
    public partial class ChangedIsPaidOfftoStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPaidOff",
                table: "Loans");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Loans",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Loans");

            migrationBuilder.AddColumn<bool>(
                name: "IsPaidOff",
                table: "Loans",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
