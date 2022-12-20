using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ASHMONEY_API.Models
{
    public class LoanRequest
    {
        public int ClientId { get; set; }
        public int Amount { get; set; }
        public int RepaymentPeriod { get; set; }
        //public decimal InterestRate { get; set; } = 5.0m; // Fixed interest rate of 5%
        //public decimal Principal { get; set; }


    }
}
