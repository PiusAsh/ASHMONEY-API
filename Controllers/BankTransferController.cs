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
            //Generating Reference number
            Random rd = new Random();
            int rand_num = rd.Next(100000000, 200000000);
            // Create a response object
            var response = new BankTransferResponse();

            // Get the sender account
            var senderAccount = GetAccount(request.SenderAccount);
            // Set the sender name and account number in the response object
            response.Sender = senderAccount.FullName;
            response.SenderAccount = senderAccount.AccountNumber.ToString();
            // Get the beneficiary account
            var beneficiaryAccount = GetAccount(request.BeneficiaryAccount);
            // Set the beneficiary name and account number in the response object
            response.Beneficiary = beneficiaryAccount.FullName;
            response.BeneficiaryAccount = beneficiaryAccount.AccountNumber.ToString();
            //Set the remaining response object
            response.TransactionDate = DateTime.Now;
            response.Amount = request.Amount;
            response.Status = "Success";
            response.ReferenceNumber = "023" + rand_num;
            response.BeneficiaryBankName = "ASHMONEY";
            response.Narration = response.Narration;


            // Calculate the new balances for the sender and recipient accounts
            var senderNewBalance = senderAccount.AccountBalance - request.Amount;
            var recipientNewBalance = beneficiaryAccount.AccountBalance + request.Amount;

            if (senderAccount == beneficiaryAccount)
            {
                return null;
            }
            // Update the balances in the database
            UpdateAccountBalance(request.SenderAccount, senderNewBalance);
            UpdateAccountBalance(request.BeneficiaryAccount, recipientNewBalance);
            return  response;
            }

        // Method for retrieving an account from the database
        private Account GetAccount(int accountNumber)
        {
            // Query the database for the account with the specified account number
            var account = _DbContext.Accounts.Where(a => a.AccountNumber == accountNumber).SingleOrDefault();
                return account;
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
                // Add the record to the database
                _DbContext.Accounts.Add(account);
                //_DbContext.Transactions.Add(account);
                // Update the changes in the database
                _DbContext.Accounts.Update(account);
                // Save the changes to the database
                _DbContext.SaveChanges();

            }  
        }

    }

}
