using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ASHMONEY_API.Models
{
    public class BankTransferResponse
    {
        [Key]
        public int TransactionId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string ReferenceNumber { get; set; }
        public string Beneficiary { get; set; }
        public string BeneficiaryAccount { get; set; }
        public string Sender { get; set; }
        public string SenderAccount { get; set; }
        public string BeneficiaryBankName { get; set; }
        public string Status { get; set; }
        public string Narration { get; set; }
        public int Amount { get; set; }
        public string Type  { get; set; }
    }
    
}

