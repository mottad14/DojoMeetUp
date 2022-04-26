using Microsoft.EntityFrameworkCore;
namespace belt_exam.Models
{ 
    // the MyContext class representing a session with our MySQL 
    // database allowing us to query for or save data
    public class MyContext : DbContext 
    { 
        public MyContext(DbContextOptions options) : base(options) { }
        // the table names will come from the DbSet variable names below
        public DbSet<User> Users { get; set; }
        public DbSet<MeetUp> MeetUps {get;set;}
        public DbSet<RSVP> RSVPs { get; set; }

    }
}
