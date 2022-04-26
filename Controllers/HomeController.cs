using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using belt_exam.Models;
//Db Manipulation and Session inclusion
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
//Brings in our password hashing
using Microsoft.AspNetCore.Identity;


namespace belt_exam.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private MyContext _context;

        public HomeController(ILogger<HomeController> logger, MyContext context)
        {
            _logger = logger;
            _context = context;
        }


        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("users/add")]
        public IActionResult AddUser(User newUser)
        {
            if(ModelState.IsValid)
            {
                if(_context.Users.Any(a => a.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email is already in use!");
                    return View("Index");
                }
                // IF USERNAME OR SOME OTHER FIELD NEEDS TO BE UNIQUE
                // if(_context.Users.Any(a => a.UserName == newUser.UserName))
                // {
                //     ModelState.AddModelError("Username", "Username is taken!");
                //     return View("Index");
                // }
                //Hashes our newUser's password
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                _context.Users.Add(newUser);
                _context.SaveChanges();
                //Saved user in db, and now setting session -- we will need to getInt32 at our dashboard and most other sites
                HttpContext.Session.SetInt32("UserId", newUser.UserId);


                return RedirectToAction("Dashboard");
            }
            return View("Index");
        }

        [HttpPost("users/login")]
        public IActionResult LogUser(LoginUser loggedIn)
        {
            if(ModelState.IsValid)
            {
                //Let's find the user, and ensure their account exists and passwords are correct
                User UserInDb = _context.Users.FirstOrDefault(a => a.Email == loggedIn.LoginEmail);
                if(UserInDb == null)
                {
                    ModelState.AddModelError("LoginEmail", "Invalid login credentials");
                    return View("Index");
                }
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(loggedIn, UserInDb.Password, loggedIn.LoginPassword);
                if(result ==0)
                {
                    ModelState.AddModelError("LoginEmail", "Invalid login credentials");
                    return View("Index");
                }
                //Setup session
                HttpContext.Session.SetInt32("UserId", UserInDb.UserId);
                return RedirectToAction("Dashboard");
            }
            return View("Index");
        }

        [HttpGet("Dashboard")]
        public IActionResult Dashboard()
        {
            if(HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.LoggedIn = _context.Users.Include(a => a.MyCreatedMeetUps).Include(b =>b.MyRSVPs).
            FirstOrDefault(a => a.UserId == HttpContext.Session.GetInt32("UserId"));
            //Gather all MeetUps to display on page
            ViewBag.AllMeetUps = _context.MeetUps.Include(b => b.Attendees)
            .ThenInclude(c => c.Guest).Include(d =>d.Planner)
            .OrderBy(u => u.MeetUpDate).ToList();
            return View();
        }

        [HttpGet("/RSVP/create/{meetUpId}")]
        public IActionResult RSVPtoMeetUp(int meetUpId)
        {
            if(HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index");
            }
            RSVP newRSVP = new RSVP {UserId = (int)HttpContext.Session.GetInt32("UserId"), MeetUpId = meetUpId};
            _context.RSVPs.Add(newRSVP);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }


        [HttpGet("RSVP/delete/{meetUpId}")]
        public IActionResult DeleteRSVP(int meetUpId)
        {
            if(HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index");
            }
            int AttendingUserId = (int)HttpContext.Session.GetInt32("UserId");
            RSVP RSVPToDel = _context.RSVPs.SingleOrDefault(a => a.MeetUpId == meetUpId && a.UserId == AttendingUserId);
            _context.RSVPs.Remove(RSVPToDel);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("/meetUp/add")]
        public IActionResult AddMeetUp()
        {
            if(HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.CurrentUserId = HttpContext.Session.GetInt32("UserId"); 
            return View("CreateMeetUp");
        }


        [HttpPost("/meetUp/create")]
        public IActionResult CreateNewMeetUp(MeetUp newMeetup)
        {
            if(HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index");
            }
            if(ModelState.IsValid)
            {
                //Let's make sure the user doesn't already have a MeetUp created 
                User UserInDb = _context.Users.Include(a => a.MyCreatedMeetUps).FirstOrDefault(a => a.UserId == newMeetup.UserId);

                //Save the MeetUp date to the Db 
                _context.MeetUps.Add(newMeetup);
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            ViewBag.CurrentUserId = HttpContext.Session.GetInt32("UserId"); 

            return View("CreateMeetUp");
        }

        [HttpGet("OneMeetUp/{ChosenMeetUpId}")]
        public IActionResult OneMeetUp(int ChosenMeetUpId)
        {
            if(HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index");
            }
            //The ViewBag now returns the Meetup Info, along with Guest Class info about the Attendees
            MeetUp ThisOneMeet = _context.MeetUps.Include(b =>b.Attendees)
            .ThenInclude(c => c.Guest).Include(d=> d.Planner)
            .FirstOrDefault(a => a.MeetUpId == ChosenMeetUpId);
            ViewBag.ThisOneMeetUp = ThisOneMeet; 



            ViewBag.LoggedIn = _context.Users.Include(a => a.MyCreatedMeetUps).Include(b =>b.MyRSVPs).
            FirstOrDefault(a => a.UserId == HttpContext.Session.GetInt32("UserId"));
             
            //View the List of those attending the specific MeetUp -- includes
            // their Info (as guests/users)
            return View("OneMeetUp", ThisOneMeet);
        }

        [HttpGet("MeetUp/delete/{MeetUpId}")]
        public IActionResult DeleteMeetUp(int MeetUpId)
        {
            if(HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index");
            }
            MeetUp MeetUpToDel = _context.MeetUps.SingleOrDefault(a => a.MeetUpId == MeetUpId);
            _context.MeetUps.Remove(MeetUpToDel);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
