using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASHMONEY_API.Models
{
    public class LoanResponse
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public decimal InterestRate { get; set; }
        public int RepaymentPeriod { get; set; }
        public decimal Principal { get; set; }
        public DateTime RepaymentDate { get; set; }
        public string BorrowerName { get; set; }
        public int BorrowerAccount { get; set; }
    }
}
