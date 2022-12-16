using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ASHMONEY_API.Models
{
    public class BankTransferRequest
    {

        public string BeneficiaryAccount { get; set; }
        public string SenderAccount { get; set; }
        public int Amount { get; set; }
    }

}
