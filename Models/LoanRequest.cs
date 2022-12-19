using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASHMONEY_API.Models
{
    public class LoanRequest
    {
        public int Amount { get; set; }
        public int BorrowerAccount { get; set; }
    }
}
