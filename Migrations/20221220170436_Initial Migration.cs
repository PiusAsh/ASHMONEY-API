using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ASHMONEY_API.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<int>(nullable: false),
                    Country = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    Gender = table.Column<string>(nullable: true),
                    DateOfBirth = table.Column<DateTime>(nullable: false),
                    AccountNumber = table.Column<int>(nullable: false),
                    BankName = table.Column<string>(nullable: true),
                    AccountBalance = table.Column<int>(nullable: false),
                    AccountType = table.Column<string>(nullable: true),
                    TransactionPin = table.Column<int>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    LastLoggedIn = table.Column<DateTime>(nullable: false),
                    Role = table.Column<string>(nullable: true),
                    Token = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Loans",
                columns: table => new
                {
                    LoanId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<int>(nullable: false),
                    ClientId = table.Column<int>(nullable: false),
                    RequestDate = table.Column<DateTime>(nullable: false),
                    InterestRate = table.Column<decimal>(nullable: false),
                    RepaymentPeriod = table.Column<int>(nullable: false),
                    Principal = table.Column<decimal>(nullable: false),
                    RepaymentDate = table.Column<DateTime>(nullable: false),
                    BorrowerName = table.Column<string>(nullable: true),
                    BorrowerAccount = table.Column<int>(nullable: false),
                    Purpose = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loans", x => x.LoanId);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionDate = table.Column<DateTime>(nullable: false),
                    ReferenceNumber = table.Column<string>(nullable: true),
                    Beneficiary = table.Column<string>(nullable: true),
                    BeneficiaryAccount = table.Column<string>(nullable: true),
                    Sender = table.Column<string>(nullable: true),
                    SenderAccount = table.Column<string>(nullable: true),
                    BeneficiaryBankName = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    Narration = table.Column<string>(nullable: true),
                    Amount = table.Column<int>(nullable: false),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Loans");

            migrationBuilder.DropTable(
                name: "Transactions");
        }
    }
}
