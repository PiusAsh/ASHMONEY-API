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

    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly AppDbContext _DbContext;

        public AccountController(AppDbContext appDbContext)
        {
            _DbContext = appDbContext;
        }




        [HttpGet]
        [Route("GetAllAccounts")]

        public async Task<IActionResult> GetAllAccounts()
          {
            return Ok(await _DbContext.Accounts.ToListAsync());
        }

       

        [HttpGet]
        [Route("GetAccountById")]
        public async Task<IActionResult> GetAccountById(int Id)
        {
            var user = await _DbContext.Accounts.FirstOrDefaultAsync(x => x.Id == Id);
            if (user == null)
            {
                return NotFound(new { Message = "User Not Found" });
            }
            else
            {
                return Ok(user);
            }
        }

        [HttpGet("GetLastLoggedInTime")]
        public DateTime GetLastLoggedInTime(int Id)
        {
            var user = _DbContext.Accounts.FirstOrDefault(u => u.Id == Id);
            return user?.LastLoggedIn ?? throw new Exception("User not found");
        }

        [HttpGet("GetLoggedInTime")]
        public void RecordLoginTime(int userId)
        {
            var user = _DbContext.Accounts.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            user.LastLoggedIn = DateTime.UtcNow;
            _DbContext.SaveChanges();
        }


        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginViewModel model)
        {
            if (model == null)
            {
                return BadRequest();
            }
            else
            {
                var user = _DbContext.Accounts.Where(a => a.Email == model.Email && a.Password == model.Password).FirstOrDefault();
                if (user != null)
                {
                    return Ok(new
                    {
                        Message = "Logged In Successfully",
                        UserData = user.Id,
                        
                        Email = user.Email
                    });
                }
                else
                {
                    return NotFound(new { Mesage = "User Does not exist" });
                }
            }
        }

        [HttpPost("Auth")]
        public async Task<IActionResult> Auth([FromBody] Account userObj)
        {
            if (userObj == null)
                return BadRequest(new { Message = "Something went wrong" });

            var user = await _DbContext.Accounts.FirstOrDefaultAsync(x => x.PhoneNumber == userObj.PhoneNumber && x.Password == userObj.Password);
            if (user == null)
                return NotFound(new { Message = "User Not Found!" });
            user.LastLoggedIn = DateTime.Now;
           await _DbContext.SaveChangesAsync();

            return Ok(new
            {
                Message = "Login success!",
                user
            });
        }




        [HttpPost("Signup")]
        public async Task<IActionResult> Signup([FromBody] Account account)
        {
            var IsEmailExist = await _DbContext.Accounts.Where(x => x.Email == account.Email).AnyAsync();

            var IsPhoneNumberExist = await _DbContext.Accounts.Where(x => x.PhoneNumber == account.PhoneNumber).AnyAsync();

            if (IsEmailExist)
            {
                return NotFound(new { Message = "Email Already Exisst" });
            }
            if (IsPhoneNumberExist)
            {
                return NotFound(new { Message = "Phone Number Already Exist" });
            }
            else
            {
                if (account == null)
                {
                    return BadRequest(new { Message = "Something went wrong" });
                }
                else
                {


                    //Random rd = new Random();
                    //int rand_num = rd.Next(1000000, 2000000);
                    //int a = 00111;
                    //int b = rand_num;

                    //int newAcctNumber = int.Parse(a.ToString() + b.ToString());

                    Random rd = new Random();
                    int uniqueNumber = rd.Next(100000000, 200000000);
                    string prefix = "002";
                    string newAcctNumber = prefix + uniqueNumber.ToString();


                    account.AccountNumber = int.Parse(newAcctNumber);
                    account.BankName = "ASHMONEY";
                    account.Token = "";
                    account.Role = "User";
                    account.AccountType = "Savings";
                    account.TransactionPin = 1234;
                    account.AccountBalance = 5000;
                    account.DateCreated = DateTime.UtcNow;
                    account.EligibleLoanAmt = 15000;
                    await _DbContext.Accounts.AddAsync(account);
                    await _DbContext.SaveChangesAsync();
                    return Ok(new
                    {
                        Message = "User Registered Successfully",
                        account
                    });
                }
            }
        }


        [HttpPut("UpdateAccount")]
        public async Task<IActionResult> UpdateAccount(int Id, Account account)
        {
            var user = await _DbContext.Accounts.FindAsync(Id);

            if (user == null)
            {
                return NotFound(new { Message = "No record Found" });
            }
            else
            {
                user.Country = account.Country;
                user.Address = account.Address;
                user.Email = account.Email;
                user.PhoneNumber = account.PhoneNumber;
                user.TransactionPin = account.TransactionPin;
                user.State = account.State;
                user.AccountType = account.AccountType;
                user.DateOfBirth = account.DateOfBirth;
                user.Password = account.Password;

              await _DbContext.SaveChangesAsync();
                return Ok(user);
            }
        }
            //[HttpDelete]
            //[Route("DeleteUser")]
            //public async Task<IActionResult> DeleteUser(int Id)
            //{
            //    var user = await _DbContext.Accounts.FindAsync(Id);
            //    if (user == null)
            //    {
            //        return NotFound();
            //    }
            //    else
            //    {
            //      await  _DbContext.Accounts.Remove(user);
            //        await _DbContext.SaveChangesAsync();
            //        return Ok(new { user = user.FirstName, Message = "User Deleted Successfully" });
            //    }


        //}
    }
    }
