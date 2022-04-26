using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace belt_exam.Models
{
    public class MeetUp
    {
        [Key]
        public int MeetUpId { get; set; }

        [Required(ErrorMessage ="Please enter a name for your Meet Up.")]
        [MinLength(2, ErrorMessage ="Name must be at least 2 characters long.")]
        [Display(Name = "Meet Up Title")]
        public string Title { get; set; }

        [Required(ErrorMessage ="Please enter a future date.")]
        [FutureDateStart(ErrorMessage ="Please enter a *future* date for your Meet Up.")]
        [Display(Name = "Date of Meet Up")] 
        public DateTime MeetUpDate { get; set; }
        [Required]
        
        [Display(Name = "Duration")] 
        public double MeetUpDuration { get; set; }
        [Required]
        public string MeetUpTimeMetric {get;set;}
        [Required]
        public string MeetUpDescription {get;set;}
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        //To begin seeing an example of a one to many relationship being built watch at 7:20 mark - https://www.youtube.com/watch?v=2PQcHUyG-MQ&list=PLTQMnrBWdoNMF9oonBgDZ1bnISlpYMhyp&index=99 
        //Connect to One to Many
        public int UserId { get; set; }
        public User Planner {get; set;}
        public List<RSVP> Attendees {get;set;}


        //Connect to Many to Many 

    }

    public sealed class FutureDateStartAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            DateTime dateStart = (DateTime)value;
            // Event must start in the future time.
            return (dateStart > DateTime.Now);
        }
    }
}
