using ASHMONEY_API.Context;
using ASHMONEY_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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


        [HttpGet("GetAllTransfer")]
        public async Task<IActionResult> GetAllTransfers()
        {
            return Ok(await _DbContext.Transactions.ToListAsync());
        }

        [HttpGet("GetByAccountNo")]
        public async Task<IActionResult> GetByAccountNo(int accountNo)
        {
            var account = await _DbContext.Accounts.FirstOrDefaultAsync(x => x.AccountNumber == accountNo);
            if(account == null)
            {
                return NotFound(new { Message = "Account Number does not exist" });
            }
            return Ok(account);
        }

            [HttpPost("Transfer")]
            public BankTransferResponse Transfer(BankTransferRequest request)
            {
                // Retrieve the sender and recipient accounts from the database
                var senderAccount = GetAccount(request.SenderAccount);
                var recipientAccount = GetAccount(request.BeneficiaryAccount);
            // Calculate the new balances for the sender and recipient accounts
            var senderNewBalance = senderAccount.AccountBalance - request.Amount;
                var recipientNewBalance = recipientAccount.AccountBalance + request.Amount;

            if (senderAccount == recipientAccount)
            {
                return null;
            }
            
                // Update the balances in the database
                UpdateAccountBalance(request.SenderAccount, senderNewBalance);
                UpdateAccountBalance(request.BeneficiaryAccount, recipientNewBalance);
            //Generating Reference number
            Random rd = new Random();
            int rand_num = rd.Next(100000000, 200000000);
            // Create and return a response object
            var response = new BankTransferResponse
                {
                    BeneficiaryAccount = request.BeneficiaryAccount.ToString(),
                    SenderAccount = request.SenderAccount.ToString(),
                    //Sender = ,
                    Amount = request.Amount,
                    Status = "Success",
                   TransactionDate = DateTime.Now,
                   ReferenceNumber =  "023" + rand_num,
                  
                };

                return  response;
            }

        // Method for retrieving an account from the database
        private Account GetAccount(int accountNumber)
        {
            // Query the database for the account with the specified account number
            var account = _DbContext.Accounts.Where(a => a.AccountNumber == accountNumber).SingleOrDefault();
                return account;
           }
        // Method for retrieving an senderName from the database
        private Account GetsenderName(string senderName)
        {
            // Query the database for the account with the specified account number
            var accountName = _DbContext.Accounts.Where(a => a.FullName == senderName).SingleOrDefault();
            return accountName;
        }



        // Method for updating an account's balance in the database
        private void UpdateAccountBalance(int accountNumber, int newBalance)
        {
            
            
                // Query the database for the account with the specified account number
                var account = _DbContext.Accounts
                    .Where(a => a.AccountNumber == accountNumber)
                    .SingleOrDefault();
            // Update the account's balance
            account.AccountBalance = newBalance;
            if(account != null)
            {
                // Update the changes in the database
                _DbContext.Accounts.Update(account);
                // Save the changes to the database
                _DbContext.SaveChanges();

            }

            
        }

        //[HttpGet]
        //public async Task<IActionResult > GetTransactionsByUser(string userId)
        //{
        //    // Query the database for all transactions made by the specified user
        //    var transactions = _DbContext.Transactions
        //      .Where(t => t.Sender == userId)
        //      .ToListAsync();

        //    return transactions;
        //}


        //[HttpGet]
        //[Route("GetAllUserOrders")]

        //public async Task<IActionResult> GetAllUserOrders(int userId)
        //{
        //    var user = await _DbContext.Transactions.FindAsync(userId);
        //    var AccountTFR = await _DbContext.Transactions.Where(x => x.TransactionId == user).Select(x => new BankTransferResponse
        //    {

        //        Status = x.Status
        //    }).ToListAsync();

        //    if (AccountTFR == null)
        //    {
        //        return NotFound(new { Message = "No Transaction found" });

        //    }

        //    return Ok(new
        //    {
        //        AccountTFR,

        //    });
        //}


    }

}
