using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ASHMONEY_API.Models
{
    public class LoanRes
    {
        [Key]
        public int LoanId { get; set; }
        public int Amount { get; set; }
        public int ClientId { get; set; }
        public DateTime RequestDate { get; set; }
        public decimal InterestRate { get; set; } = 5.0m; // Fixed interest rate of 5%
        public int RepaymentPeriod { get; set; }
        public decimal Principal { get; set; }
        public DateTime RepaymentDate { get; set; }
        public string BorrowerName { get; set; }
        public int BorrowerAccount { get; set; }
        public string Purpose { get; set; }
    }
}
