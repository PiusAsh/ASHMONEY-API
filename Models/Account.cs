using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ASHMONEY_API.Models
{
    public class Account
    {
        [Key]
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int PhoneNumber { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string Address { get; set; }
        public string Password { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int AccountNumber { get; set; }
        public string BankName { get; set; }
        public int AccountBalance { get; set; }
        public string AccountType { get; set; }
        public int TransactionPin { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime LastLoggedIn { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }

    }
}
