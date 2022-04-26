using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace belt_exam.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required(ErrorMessage ="Please enter a first name")]
        [MinLength(2, ErrorMessage ="First Name must be at least 2 characters long.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required(ErrorMessage ="Please enter a last name")]
        [MinLength(2, ErrorMessage ="Last Name must be at least 2 characters long.")]
        [Display(Name = "Last Name")] 
        public string LastName { get; set; }
        [Required(ErrorMessage ="Please enter a valid email")]
        [EmailAddress(ErrorMessage ="Please enter a valid email")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [RegularExpression("^.*(?=.{8,})(?=.*)(?=.*[a-z])(?=.*)(?=.*[@#!$%^&+=]).*$", ErrorMessage="Password must contain atleast 1 number, 1 letter, and 1 of the following special characters: @#$%^&+=")]
        [MinLength(8, ErrorMessage ="Password must be at least 8 characters long.")]
        [Display(Name = "Password must contain atleast 1 number, 1 letter, and 1 of the following special characters: @#$%^&+=")] 
        public string Password { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        //To begin seeing an example of a one to many relationship being built watch at 7:20 mark - https://www.youtube.com/watch?v=2PQcHUyG-MQ&list=PLTQMnrBWdoNMF9oonBgDZ1bnISlpYMhyp&index=99 
        //List of events I created
        public List<MeetUp> MyCreatedMeetUps {get; set;}
        
        //Connect The List of Events I want to attend 
        public List<RSVP> MyRSVPs {get; set;}



        [NotMapped]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string Confirm {get;set;}
    }
}
