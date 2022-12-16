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

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(Account userObj)
        {
            var IsEmailExist = await _DbContext.Accounts.Where(x => x.Email == userObj.Email).AnyAsync();

            var IsPhoneNumberExist = await _DbContext.Accounts.Where(x => x.PhoneNumber == userObj.PhoneNumber).AnyAsync();

            if (IsEmailExist)
            {
                return NotFound(new { Message = "Email Already Exisst" });
            }
            if (IsPhoneNumberExist)
            {
                return NotFound(new { Message = "Phone Number Already Exisst" });
            }
            else
            {
                if (userObj == null)
                {
                    return BadRequest(new { Message = "Something went wrong" });
                }
                else
                {

                    //foreach (var userReg in userObj.AccountNumber)
                    //{
                    Random rd = new Random();
                    int rand_num = rd.Next(1000000, 2000000);

                    var user = new Account()
                    {
                        Address = userObj.Address,
                        Email = userObj.Email,
                        PhoneNumber = userObj.PhoneNumber,
                        Password = userObj.Password,
                        DateOfBirth = userObj.DateOfBirth,
                        Gender = userObj.Gender,
                        Country = userObj.Country,
                        FirstName = userObj.FirstName,
                        LastName = userObj.LastName,
                        State = userObj.State,
                        
                        AccountNumber = "001" + rand_num,
                        AccountType = userObj.AccountType,
                        Role = userObj.Role,
                        Token = userObj.Token,
                        TransactionPin = userObj.TransactionPin,
                        BankName = "ASHMONEY",
                        AccountBalance = "5000"

                    };

                    await _DbContext.Accounts.AddAsync(user);

                    await _DbContext.SaveChangesAsync();
                    return Ok(new
                    {
                        Message = "Successfully Registered",
                        UserData = userObj
                    });
                }
            }

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
                return NotFound(new { Message = "Phone Number Already Exisst" });
            }
            else
            {
                if (account == null)
                {
                    return BadRequest(new { Message = "Something went wrong" });
                }
                else
                {


                    Random rd = new Random();
                    int rand_num = rd.Next(1000000, 2000000);

                    account.AccountNumber = "001" + rand_num;
                    account.BankName = "ASHMONEY";
                    account.Token = "";
                    account.Role = Account.UserRole.User;
                    account.AccountType = Account.AcctType.Savings;
                    account.AccountBalance = "5000";
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
