using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace belt_exam.Models
{
    public class RSVP
    {
        [Key]
        public int RSVPid { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]  
        public int MeetUpId { get; set; }
        public User Guest {get;set;}
        public MeetUp UpcomingMeetUp {get;set;}
        public DateTime CreatedAt {get; set;} = DateTime.Now;
        public DateTime UpdatedAt {get; set;} = DateTime.Now;
    }
}