using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BetaAirlinesMVC.Utilities;

namespace BetaAirlinesMVC.Controllers
{
    [SessionCheck]
    // Uses BetaAirlinesMVC.Utilities to run a SessionCheck
    // Having it here runs the session check in all actions on this controller
    // Else place it only on the actions that you want it on
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            System.Diagnostics.Debug.WriteLine("test");
            return View();
        }
    }
}