using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BetaAirlinesMVC.Models;
using BetaAirlinesMVC.ViewModel;
using BetaAirlinesMVC.Utilities;

namespace BetaAirlinesMVC.Controllers
{
    // Do NOT have Session Check on entire controller to avoid infinite loop of Login Action    
    public class UsersController : Controller
    {
        private BetaAirlinesDbContext db = new BetaAirlinesDbContext();


        // GET: Users
        // Uses BetaAirlinesMVC.Utilities to run a SessionCheck
        // Having it here runs the session check in all actions on this controller
        // Else place it only on the actions that you want it on

        public ActionResult Index()
        {
            var users = db.Users.Include(u => u.UserRole).OrderBy(x => x.FirstName);
            return View(users.ToList());
            // return View();
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            // Get the list of active user roles for the user
            var userRoles = db.UserRoles.Where(e => e.Id == id && e.Active == 1).ToList();

            // Create the list of User Roles
            List<UserRole> roles = new List<UserRole>();

            foreach (var userRole in userRoles)
            {
                UserRole urm = new UserRole
                {
                    Role = userRole.Role,
                    Description = userRole.Description
                };

                // Add to the roles list
                roles.Add(urm);
            }

            // Pass the user and roles to the view
            ViewBag.UserRoles = roles;

            return View(user);
        }


        // GET: Users/Create
        public ActionResult Create()
        {

            UserCreateViewModel model = new UserCreateViewModel();

            // set default values for the form
            model.RegisteredDate = DateTime.Today;
            model.Active = 1;
            
            // Name the item in the viewbag whatever you'd like. ViewBag.[CustomName]
            var userroleslist = db.UserRoles.ToList();
            SelectList list = new SelectList(userroleslist, "Id", "Role");
            ViewBag.UserRoleList = list;
            return View(model);
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FirstName,LastName,Username,Password,ConfirmPassword,RegisteredDate,Active,UserRoleID")] UserCreateViewModel userCreateViewModel)
        {
            if (ModelState.IsValid)
            {
                DataValidation dataValidation = new DataValidation();
                if (dataValidation.UsernameAlreadyExists(userCreateViewModel.Username))
                {
                    ViewBag.Message = "Username already exists.";
                    ViewBag.UserRoleList = new SelectList(db.UserRoles, "Id", "Role");
                    return View(userCreateViewModel);
                }

                User user = new User
                {
                    Username = userCreateViewModel.Username,
                    FirstName = userCreateViewModel.FirstName,
                    LastName = userCreateViewModel.LastName,
                    Password = userCreateViewModel.Password,
                    RegisteredDate = DateTime.Today,
                    Active = userCreateViewModel.Active,
                    UserRoleID = userCreateViewModel.UserRoleID
                };

                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            ViewBag.UserRoleList = new SelectList(db.UserRoles, "Id", "Role");
            return View(userCreateViewModel);
        }


        // GET: Users/Edit/5
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserRoleID = new SelectList(db.UserRoles, "Id", "Role", user.UserRoleID);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,Username,Password,RegisteredDate,Active,UserRoleID")] User user)
        {
            DataValidation dv = new DataValidation();

            if (ModelState.IsValid)
            {
                if(!dv.PWMatch(user.Id, user.Password))
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                }

                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserRoleID = new SelectList(db.UserRoles, "Id", "Role", user.UserRoleID);
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // GET: Users/Login
        [HttpGet]
        public ActionResult Login()
        {
            // If already logged in, then log out first. 
            if(Session["id"] != null || Session["pw"] != null) {
                Logout();
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string userName, string password)
        {
            UserLoginViewModel model = new UserLoginViewModel();

            if (userName == null || password == null)
            {
                ViewBag.Message = "Please enter the username and password";
                return View();
            }

            // Get user loging information from the database
            User user = db.Users.Where(x => x.Username == userName).SingleOrDefault();
            if (user == null)
            {
                ViewBag.Message = "Invalid Username or Password";
                return View();
            }
            UserRole userRole = db.UserRoles.Where(x => x.Id == user.UserRoleID).SingleOrDefault();
            
            DataValidation authenticate = new DataValidation();
            bool isVerified = authenticate.IsAuthenticated(user.Id, password);
            if (isVerified)
            {
                this.Session["id"] = user.Id;
                this.Session["pw"] = user.Password;
                this.Session["role"] = userRole.Role;

                if(userRole.Role == "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }
                else if(userRole.Role == "User")
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("BookAFlight", "Flights");
                }
                
            } else
            {
                ViewBag.Message = "Invalid Username or Password";
                this.Session.Clear();
            }

            return View(model);
        }


        [HttpGet]
        public ActionResult Logout()
        {
            Session["id"] = null;
            Session["pw"] = null;
            Session["role"] = null;
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
            Response.AddHeader("Cache-control", "no-store, must-revalidate, private, no-cache");
            Response.AddHeader("Pragma", "no-cache");
            Response.AddHeader("Expires", "0");
            Response.AppendToLog("window.location.reload();");

            return RedirectToAction("Login", "Users");
        }
    }
}
