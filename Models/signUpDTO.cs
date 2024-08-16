using System.ComponentModel.DataAnnotations;
using System;

namespace ASHMONEY_API.Models
{
    public class SignupDTO
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Gender { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
    }

}
