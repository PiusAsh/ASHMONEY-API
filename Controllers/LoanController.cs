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

        //[HttpPost("LoanRequest")]
        //public async Task<IActionResult> LoanRequest(LoanRequest request)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    // Calculate the principal
        //    decimal principal = request.LoanAmount * (1 + request.InterestRate * request.RepaymentPeriod / 12);
        //    request.Principal = principal;

        //    _DbContext.LoanRequests.Add(request);
        //    _DbContext.SaveChanges();

        //    return Ok();
        //}

    }
}
