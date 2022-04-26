using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace belt_exam.Models
{
    public class LoginUser
    {
        // No other fields!
        [Required(ErrorMessage ="Please enter your email")]
        public string LoginEmail {get; set;}
        [Required(ErrorMessage ="Please enter your password")]
        [DataType(DataType.Password)]
        public string LoginPassword { get; set; }

    }
}