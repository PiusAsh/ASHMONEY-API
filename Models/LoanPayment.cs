using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASHMONEY_API.Models
{
    public class LoanPayment
    {
        public int LoanId { get; set; }
        public decimal Amount { get; set; }
    }
}
