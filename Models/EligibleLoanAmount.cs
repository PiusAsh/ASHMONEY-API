using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASHMONEY_API.Models
{
    public class EligibleLoanAmount
    {
        public int Calculate(int balance)
        {
            if (balance >= 5000)
            {
                return  10000;
            }
            else if (balance <= 10000)
            {
                return 50000;
            }
            else if (balance >= 10000)
            {
                return 100000;
            }
            else if (balance >= 50000)
            {
                return 200000;
            }
            else if (balance >= 100000)
            {
                return 1000000;
            }
            else if (balance >= 1000000)
            {
                return 10000000;
            }
            return 0;
        }
    }
}
