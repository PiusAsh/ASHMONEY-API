using ASHMONEY_API.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        const int MIN_REPAYMENT_PERIOD = 1;
        const int MAX_REPAYMENT_PERIOD = 6;

        [HttpGet("GetLoans")]
        public async Task<IActionResult> GetLoans()
        {
            // Query the database for all loans made by the user
            var loans = await _DbContext.Loans.ToListAsync();

            return Json(loans);
        }


        [HttpGet("GetLoansByClient")]
        public async Task<IEnumerable<LoanResponse>> GetLoansByClient(int clientId)
        {
            // Query the database for all loans made by the borrower
            var loans = await _DbContext.Loans
                .Where(l => l.ClientId == clientId).OrderByDescending(x => x.LoanId)
                .ToListAsync();

            return loans;
        }


        [HttpPost ("LoanRequest")]
        public IActionResult LoanRequest(LoanRequest loanRequest)
        {
            // Get the client account from the database
            var clientAccount = _DbContext.Accounts.SingleOrDefault(a => a.Id == loanRequest.ClientId);

            // Create a new LoanRequest object with the provided data
            LoanResponse response = new LoanResponse();

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
            UpdateAccountBalance(response.BorrowerAccount, BorrowerNewBalance, response);




            return Ok(new { Message = "Loan Approved and Disbursed Successfully", response });
        }
        // Method for updating an account's balance in the database
        private void UpdateAccountBalance(int accountNumber, int newBalance, LoanResponse res)
        {
            // Query the database for the account with the specified account number
            var account = _DbContext.Accounts
                .Where(a => a.AccountNumber == accountNumber)
                .SingleOrDefault();
            // Update the account's balance
            account.AccountBalance = newBalance;
            if (account != null)
            {


                var data = new LoanResponse();
                data.BorrowerName = account.FullName;
                data.BorrowerAccount = account.AccountNumber;
                data.ClientId = account.Id;
                data.InterestRate = res.InterestRate;
                //data.Principal = res.Principal;
                data.Purpose = res.Purpose;
                data.RepaymentPeriod = res.RepaymentPeriod;
                //data.RepaymentDate = res.RepaymentDate;
                data.RequestDate = DateTime.Now;
                data.Amount = res.Amount;
                data.LoanId = res.LoanId;
                data.AmountPaid = +res.AmountPaid;
                // Check if the user has paid off their loan
                if (data.AmountPaid == res.Principal)
                {
                    data.Status = "Paid";
                }




                // Calculate the due date based on the repayment period
                DateTime dueDate = DateTime.Now.AddMonths(data.RepaymentPeriod);

                // Calculate the principal
                decimal principal = data.Amount * (1 + data.InterestRate * data.RepaymentPeriod / 12);

                data.Principal = principal;
                data.RepaymentDate = dueDate;

                
                // Update the changes in the database
                _DbContext.Loans.Update(data);
                _DbContext.Loans.Add(data);
                _DbContext.Accounts.Update(account);
                // Save the changes to the database
                _DbContext.SaveChanges();

            }
        }





        [HttpPost("MakeLoanPayment")]
        public IActionResult MakeLoanPayment(LoanPayment payment)
         {
            // Validate the input data
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Query the database for the loan with the specified loan ID
            var loan = _DbContext.Loans
                .Where(l => l.LoanId == payment.LoanId)
                .SingleOrDefault();

            // Check if the loan exists
            if (loan == null)
            {
                return BadRequest("The specified loan does not exist.");
            }

            // Check if the loan has already been paid off
            if (loan.Status == "Paid")
            {
                return BadRequest("This loan has already been paid off.");
            }

            //Check if the payment amount is less than the remaining balance on the loan
            if (payment.Amount < loan.Principal)
            {
                return BadRequest("Your part payment has been received. Please pay the balance before due date");
            }

            // Calculate the new remaining balance on the loan
            //if (loan.AmountPaid < 0)
            //{
            //    loan.AmountPaid = loan.AmountPaid * -1;
            //}
            decimal newBalance = loan.AmountPaid + payment.Amount;

            loan.AmountPaid = newBalance;
            if (loan.Principal == loan.AmountPaid)
            {
                loan.Status = "Paid";
            }
            _DbContext.Loans.Update(loan);
            _DbContext.SaveChanges();

            // Retrieve the borrower's account information from the database
            var borrowerAccount = _DbContext.Accounts
                .Where(a => a.AccountNumber == loan.BorrowerAccount)
                .FirstOrDefault();
            if (borrowerAccount == null)
            {
                return NotFound();
            }
            // Calculate the new balance on the borrower's account
            if (borrowerAccount.AccountBalance < 0) {
                borrowerAccount.AccountBalance = borrowerAccount.AccountBalance * -1;
            }

            if (borrowerAccount.AccountBalance < payment.Amount)
            {
                return BadRequest("Insufficient Balance. Please fund your account and try again");
            }
            decimal newAccountBalance = borrowerAccount.AccountBalance - payment.Amount;
            var balance = Convert.ToDecimal(newBalance);
            // Update the borrower's account balance in the database
            borrowerAccount.AccountBalance = (int)newAccountBalance;
            _DbContext.Accounts.Update(borrowerAccount);
            _DbContext.SaveChanges();           
            // Return an OK response with a success message
            return Ok(new { Message = "Loan payment successful.", loan });
        }





    }
}
