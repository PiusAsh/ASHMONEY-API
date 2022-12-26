using ASHMONEY_API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASHMONEY_API.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base (options)
        {

        }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<BankTransferResponse> Transactions { get; set; }
        public DbSet<LoanResponse> Loans { get; set; }
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Account>().ToTable("Accounts");
        //    modelBuilder.Entity<BankTransferResponse>().ToTable("Transactions");
        //    modelBuilder.Entity<LoanRes>().ToTable("LoanRequest").Property(p => p.Principal)
        //.HasColumnType("decimal(18, 2)");
        //    modelBuilder.Entity<LoanRes>()
        //.Property(p => p.InterestRate)
        //.HasColumnType("decimal(18, 2)");
        //}
    }
}
