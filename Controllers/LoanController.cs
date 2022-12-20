using ASHMONEY_API.Context;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASHMONEY_API.Models
{
    public class LoanController : Controller
    {
        private readonly AppDbContext _DbContext;

        public LoanController(AppDbContext appDbContext)
        {
            _DbContext = appDbContext;
        }

        const int MIN_LOAN_AMOUNT = 5000;
        const int MAX_LOAN_AMOUNT = 1000000;
        const int MIN_REPAYMENT_PERIOD = 2;
        const int MAX_REPAYMENT_PERIOD = 6;
        const int MIN_CREDIT_SCORE = 600;


        [HttpPost ("LoanRequest")]
        public IActionResult LoanRequest(LoanRequest loanRequest)
        {
            // Get the client account from the database
            var clientAccount = _DbContext.Accounts.SingleOrDefault(a => a.Id == loanRequest.ClientId);

            // Create a new LoanRequest object with the provided data
            LoanRes response = new LoanRes();

            response.Amount = loanRequest.Amount;
            response.BorrowerAccount = clientAccount.AccountNumber;
            response.ClientId = loanRequest.ClientId;
            response.BorrowerName = clientAccount.FullName;
            response.InterestRate = response.InterestRate;
            response.RepaymentDate = response.RepaymentDate;
            response.RepaymentPeriod = loanRequest.RepaymentPeriod;
            response.Principal = response.Principal;
            response.RequestDate = DateTime.Now;


            // Validate the input data
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if the loan amount is within the allowable range
            if (loanRequest.Amount < MIN_LOAN_AMOUNT || loanRequest.Amount > MAX_LOAN_AMOUNT)
            {
                return BadRequest("The loan amount must be between ₦" + MIN_LOAN_AMOUNT + " and ₦" + MAX_LOAN_AMOUNT);
            }

            // Check if the repayment period is within the allowable range
            if (loanRequest.RepaymentPeriod < MIN_REPAYMENT_PERIOD || loanRequest.RepaymentPeriod > MAX_REPAYMENT_PERIOD)
            {
                return BadRequest("The repayment period must be between " + MIN_REPAYMENT_PERIOD + " months and " + MAX_REPAYMENT_PERIOD + " months.");
            }
            // Check if the client has a good credit score
            if (clientAccount.AccountBalance < 5000)
            {
                return BadRequest("Your credit score is too low to qualify for a loan.");
            }
            // Calculate the principal
            decimal principal = loanRequest.Amount * (1 + response.InterestRate * loanRequest.RepaymentPeriod / 12);
            response.Principal = principal;
            // Calculate the new balances for the sender and recipient accounts
            var BorrowerNewBalance = clientAccount.AccountBalance + loanRequest.Amount;
            // Update the balances in the database
            UpdateAccountBalance(response.BorrowerAccount, BorrowerNewBalance);
            
            return Ok(new { Message = "Loan Approved and Disbursed Successfully", response });
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
            if (account != null)
            {
                // Update the changes in the database
                _DbContext.Accounts.Update(account);
                // Save the changes to the database
                _DbContext.SaveChanges();

            }
        }


    }
}
