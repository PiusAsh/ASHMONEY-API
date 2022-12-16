using ASHMONEY_API.Context;
using ASHMONEY_API.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASHMONEY_API.Controllers
{
    public class BankTransferController : Controller
    {
        private readonly AppDbContext _DbContext;

        public BankTransferController(AppDbContext appDbContext)
        {
            _DbContext = appDbContext;
        }

        //[HttpPost("Transfer")]
        //public async Task<IActionResult> Transfer()
        //{

        //}
        




            [HttpPost("Transfer")]
            public BankTransferResponse Transfer(BankTransferRequest request)
            {
                // Retrieve the sender and recipient accounts from the database
                var senderAccount = GetAccount(request.SenderAccount);
                var recipientAccount = GetAccount(request.BeneficiaryAccount);

            int senderAcct = Int32.Parse(senderAccount.AccountBalance);
            int beneBalance = Int32.Parse(recipientAccount.AccountBalance);
            // Calculate the new balances for the sender and recipient accounts
            var senderNewBalance = senderAcct - request.Amount;
                var recipientNewBalance = beneBalance + request.Amount;

                // Update the balances in the database
                UpdateAccountBalance(request.SenderAccount, senderNewBalance);
                UpdateAccountBalance(request.BeneficiaryAccount, recipientNewBalance);
            //Generating Reference number
            Random rd = new Random();
            int rand_num = rd.Next(100000000, 200000000);
            // Create and return a response object
            var response = new BankTransferResponse
                {
                    
                    Status = "Success",
                   TransactionDate = DateTime.Now,
                   ReferenceNumber = "ASH" + rand_num,
                  
                };

                return response;
            }

        // Method for retrieving an account from the database
        private Account GetAccount(string accountNumber)
        {
            // Query the database for the account with the specified account number
            var account = _DbContext.Accounts.Where(a => a.AccountNumber == accountNumber).SingleOrDefault();
                return account;
           }
        


        // Method for updating an account's balance in the database
        private void UpdateAccountBalance(string accountNumber, int newBalance)
        {
            
            
                // Query the database for the account with the specified account number
                var account = _DbContext.Accounts
                    .Where(a => a.AccountNumber == accountNumber)
                    .SingleOrDefault();
            int AcctBalance = Int32.Parse(account.AccountBalance);
            // Update the account's balance
            AcctBalance = newBalance;

                // Save the changes to the database
               _DbContext.SaveChanges();
            
        }




    }

}
