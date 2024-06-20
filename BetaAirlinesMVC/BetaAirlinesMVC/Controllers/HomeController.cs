using BetaAirlinesMVC.Models;
using BetaAirlinesMVC.Utilities;
using BetaAirlinesMVC.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace BetaAirlinesMVC.Controllers
{
    public class HomeController : Controller
    {
        private BetaAirlinesDbContext db = new BetaAirlinesDbContext();
        private DataValidation dv = new DataValidation();

        // GET: Flights
        public ActionResult Index()
        {
            ViewBag.dpt = new SelectList(db.Airports, "Id", "Name"); // Departure
            ViewBag.arr = new SelectList(db.Airports, "Id", "Name"); // Arrival
            ViewBag.UserID = Session["id"];
            var flights = db.Flights.Include(f => f.ArrivalAirport).Include(f => f.DepartureAirport).OrderBy(f => f.DepartureDate);
            return View(flights.ToList());
        }
        [HttpPost]


        //used to search flights in home page
        public ActionResult Index(FlightSearchViewModel model)
        {
            ViewBag.dpt = new SelectList(db.Airports, "Id", "Name"); // Departure
            ViewBag.arr = new SelectList(db.Airports, "Id", "Name"); // Arrival
            ViewBag.UserID = Session["id"];
            if (ModelState.IsValid)
            {
                var results = db.Flights
                    .Where(f => f.DepartureAirport.Name.Contains(model.Location) &&
                                f.DepartureDate >= model.Departure &&
                                f.DepartureDate <= model.Return)
                    .ToList();

                model.SearchResults = results;
            }

            return View(model);
        }
        public ActionResult Plan()
        {

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}