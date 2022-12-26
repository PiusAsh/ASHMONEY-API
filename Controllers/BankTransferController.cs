using ASHMONEY_API.Context;
using ASHMONEY_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Net.Http;

namespace ASHMONEY_API.Controllers
{
    public class BankTransferController : Controller
    {
        private readonly AppDbContext _DbContext;
        int clear = 0;

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

        [HttpGet("GetUserTransaction")]
        public async Task<IEnumerable<BankTransferResponse>> GetUserTransaction(string accountNumber)
        {
            
            // Query the database for all transfers made by the user
            var transfers =  _DbContext.Transactions.Where(t => t.SenderAccount == accountNumber || t.BeneficiaryAccount == accountNumber)
                .ToList().OrderByDescending(x => x.TransactionId);

            return transfers;
        }


        

[HttpPost("Transfer")]
    public async Task<ActionResult<BankTransferResponse>> Transfer(BankTransferRequest request)
    {
        try
        {
            // Generating Reference number
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
            if (beneficiaryAccount == null)
            {
                return BadRequest("Beneficiary account not found");
            }
            // Set the beneficiary name and account number in the response object
            response.Beneficiary = beneficiaryAccount.FullName;

            response.BeneficiaryAccount = beneficiaryAccount.AccountNumber.ToString();
            //Set the remaining response object
            response.TransactionDate = DateTime.Now;
            response.Amount = request.Amount;
            response.Status = "Success";
            response.ReferenceNumber = "001" + rand_num;
            response.BeneficiaryBankName = "ASHMONEY";
            response.Narration = response.Narration;
            // Set the transaction type
            if (request.Amount > 0)
            {
                response.Type = "Credit";
            }
            else if (request.Amount < 0)
            {
                response.Type = "Debit";
            }
            else
            {
                // Amount is 0
                response.Status = "Failed";
            }

            // Calculate the new balances for the sender and recipient accounts
            var senderNewBalance = senderAccount.AccountBalance - request.Amount;
            var recipientNewBalance = beneficiaryAccount.AccountBalance + request.Amount;

            if (senderAccount == beneficiaryAccount)
            {
                return BadRequest("Both accounts cannot be the same");
            }
            if (senderAccount.AccountBalance < request.Amount)
            {
                return BadRequest("Insufficient balance");
            }
            // Update the balances in the database
            await UpdateAccountBalance(request.SenderAccount, senderNewBalance, response);
            await UpdateAccountBalance(request.BeneficiaryAccount, recipientNewBalance, response);
            return response;
        }
        catch (DbUpdateException e)
        {
                // code to provide feedback to the user that there is an issue with the database
                return BadRequest("An error occurred while making the HTTP request");
        }
        catch (HttpRequestException e)
        {
                // code to provide feedback to the user that there is a connection error
                return BadRequest("Internet Connection error");
            }
        catch (Exception e)
        {

                return BadRequest("Unable to complete this transaction");
                // code to provide feedback to the user that there is an issue with the request
            }
    }




    [HttpPost("Transfers")]
        public async Task<ActionResult<BankTransferResponse>> Transfers(BankTransferRequest request)
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
            if (beneficiaryAccount == null)
            {
                return BadRequest("Beneficiary account not found");
            }
            // Set the beneficiary name and account number in the response object
            response.Beneficiary = beneficiaryAccount.FullName;
            
            response.BeneficiaryAccount = beneficiaryAccount.AccountNumber.ToString();
            //Set the remaining response object
            response.TransactionDate = DateTime.Now;
            response.Amount = request.Amount;
            response.Status = "Success";
            response.ReferenceNumber = "001" + rand_num;
            response.BeneficiaryBankName = "ASHMONEY";
            response.Narration = response.Narration;
            // Set the transaction type
            if (request.Amount > 0)
            {
                response.Type = "Credit";
            }
            else if (request.Amount < 0)
            {
                response.Type = "Debit";
            }
            else
            {
                // Amount is 0
                response.Status = "Failed";
            }

            // Calculate the new balances for the sender and recipient accounts
            var senderNewBalance = senderAccount.AccountBalance - request.Amount;
            var recipientNewBalance = beneficiaryAccount.AccountBalance + request.Amount;

            if (senderAccount == beneficiaryAccount)
            {
                return BadRequest("Both accounts cannot be the same");
            }
            if (senderAccount.AccountBalance < request.Amount)
            {
                return BadRequest("Insufficient balance");
            }
            // Update the balances in the database
            await UpdateAccountBalance(request.SenderAccount, senderNewBalance, response);
            await UpdateAccountBalance(request.BeneficiaryAccount, recipientNewBalance, response);
            return  response;
            }

        // Method for retrieving an account from the database
        private Account GetAccount(int accountNumber)
        {
            // Query the database for the account with the specified account number
            var account = _DbContext.Accounts.Where(a => a.AccountNumber == accountNumber).SingleOrDefault();
                return account;
           }

        // Method for updating the account's balance in the database
        private async Task<int> UpdateAccountBalance(int accountNumber, int newBalance, BankTransferResponse response)
        {
            // Query the database for the account with the specified account number
            int oldBalance = 0;
            int resP = 0;

                var account = _DbContext.Accounts
                    .Where(a => a.AccountNumber == accountNumber)
                    .SingleOrDefault();
            // Update the account's balance
            
            if(account != null)
            {
                oldBalance = account.AccountBalance;
                account.AccountBalance = newBalance;
                // Update the changes in the database
                _DbContext.Accounts.Update(account);
                // Save the changes to the database
               int res  = await  _DbContext.SaveChangesAsync();
                if(res > 0)
                {
                    if(clear == 0)
                    {
                        var data = new BankTransferResponse()
                        {
                            TransactionDate = DateTime.Now,
                            ReferenceNumber = response.ReferenceNumber,
                            Beneficiary = response.Beneficiary,
                            Sender = response.Sender,
                            Status = "Success",
                            Narration = response.Narration,
                            Amount = response.Amount,
                            BeneficiaryAccount = response.BeneficiaryAccount,
                            SenderAccount = response.SenderAccount,
                            BeneficiaryBankName = "ASHMONEY"
                        };

                        _DbContext.Transactions.Add(data);
                        int res2 = await _DbContext.SaveChangesAsync();
                        if (res2 > 0)
                        {
                            clear = 1;
                            resP = 0;
                        }
                        else
                        {
                            //account.AccountBalance = oldBalance;
                            _DbContext.Accounts.Update(account);
                            await _DbContext.SaveChangesAsync();

                            resP = -1;
                        }
                    }
                }
                else
                {
                    resP = - 1;
                }
            } 
            else
            {
                resP = -1;
            }

            return resP;
        }

    }

}
