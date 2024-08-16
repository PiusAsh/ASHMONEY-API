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


        [HttpPost("LoanRequest")]
        public IActionResult LoanRequest(LoanRequest loanRequest)
        {
            var clientAccount = _DbContext.Accounts.SingleOrDefault(a => a.Id == loanRequest.ClientId);

            if (clientAccount == null)
            {
                return NotFound(new { Message = "Account not found. Please check and try again" });
            }

            if (clientAccount.AccountNumber == null)
            {
                return NotFound(new { Message = "Account number not found. Please check and try again" });
            }

            // Assuming you set the InterestRate in the LoanResponse
            decimal interestRate = 0.03m; // 3% annual interest rate

            // Create a new LoanRequest object with the provided data
            LoanResponse response = new LoanResponse
            {
                Amount = loanRequest.Amount,
                BorrowerAccount = clientAccount.AccountNumber,
                ClientId = loanRequest.ClientId,
                BorrowerName = clientAccount.FullName,
                InterestRate = interestRate, // Set the interest rate
                RepaymentDate = DateTime.Now.AddMonths(loanRequest.RepaymentPeriod),
                RepaymentPeriod = loanRequest.RepaymentPeriod,
                Principal = 0, // To be calculated
                RequestDate = DateTime.Now
            };

            // Validate the input data
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            EligibleLoanAmount eligibleLoanAmount = new EligibleLoanAmount();
            clientAccount.EligibleLoanAmt = eligibleLoanAmount.Calculate(clientAccount.AccountBalance);

            if (loanRequest.Amount > clientAccount.EligibleLoanAmt)
            {
                return BadRequest(new { Message = "The loan amount must not exceed ₦" + clientAccount.EligibleLoanAmt + " for now" });
            }

            if (loanRequest.RepaymentPeriod < MIN_REPAYMENT_PERIOD || loanRequest.RepaymentPeriod > MAX_REPAYMENT_PERIOD)
            {
                return BadRequest("The repayment period must be between " + MIN_REPAYMENT_PERIOD + " months and " + MAX_REPAYMENT_PERIOD + " months.");
            }

            if (clientAccount.AccountBalance < 5000)
            {
                return BadRequest("Your credit score is too low to qualify for a loan.");
            }

            // Calculate the principal
            decimal principal = loanRequest.Amount * (1 + (interestRate * loanRequest.RepaymentPeriod / 12));
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
            var account = _DbContext.Accounts
                .Where(a => a.AccountNumber == accountNumber)
                .SingleOrDefault();

            if (account != null)
            {
                account.AccountBalance = newBalance;

                var data = new LoanResponse
                {
                    BorrowerName = account.FullName,
                    BorrowerAccount = account.AccountNumber,
                    ClientId = account.Id,
                    InterestRate = res.InterestRate,
                    Principal = res.Principal,
                    RepaymentPeriod = res.RepaymentPeriod,
                    RequestDate = DateTime.Now,
                    Amount = res.Amount,
                    LoanId = res.LoanId,
                    AmountPaid = res.AmountPaid,
                    Status = res.AmountPaid == res.Principal ? "Paid" : "Not Paid",
                    RepaymentDate = DateTime.Now.AddMonths(res.RepaymentPeriod)
                };

                _DbContext.Loans.Update(data);
                _DbContext.Accounts.Update(account);
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

            // Retrieve the borrower's account information from the database
            var borrowerAccount = _DbContext.Accounts
                .Where(a => a.AccountNumber == loan.BorrowerAccount)
                .FirstOrDefault();

            if (borrowerAccount == null)
            {
                return NotFound("Borrower's account not found.");
            }

            // Check if the payment amount is less than the remaining balance on the loan
            decimal remainingLoanAmount = loan.Principal - loan.AmountPaid;

            if (payment.Amount < remainingLoanAmount)
            {
                loan.AmountPaid += payment.Amount;
                borrowerAccount.AccountBalance -= (int)payment.Amount;
                loan.Status = "Not Paid"; // Ensure status reflects the unpaid loan
                _DbContext.Loans.Update(loan);
                _DbContext.Accounts.Update(borrowerAccount);
                _DbContext.SaveChanges();

                return Ok(new { Message = "Your part payment has been received. Please pay the remaining balance before the due date.", loan });
            }
            else
            {
                // Payment exceeds or matches the remaining balance
                decimal excessAmount = payment.Amount - remainingLoanAmount;

                loan.AmountPaid = loan.Principal; // Mark the loan as fully paid
                loan.Status = "Paid";
                borrowerAccount.AccountBalance += (int)excessAmount ;
                // Calculate the amount to increase (50% of the initial value)
                int initialEligibleLoanAmt = borrowerAccount.EligibleLoanAmt; // Assuming this is an integer
                int amountToIncrease = (int)(initialEligibleLoanAmt * 0.50m);
                borrowerAccount.EligibleLoanAmt += amountToIncrease;

                _DbContext.Loans.Update(loan);
                _DbContext.Accounts.Update(borrowerAccount);
                _DbContext.SaveChanges();

                string message = excessAmount > 0
                    ? $"Loan payment successful. Excess amount of {excessAmount:C} has been credited back to your account."
                    : "Loan payment successful.";

                return Ok(new { Message = message, loan });
            }
        }





    }
}
