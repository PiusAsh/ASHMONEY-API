using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASHMONEY_API.Models
{
    public class CombinedTransaction
    {
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public string Sender { get; set; }
        public string Beneficiary { get; set; }
        public string ReferenceNumber { get; set; }
    }

}
