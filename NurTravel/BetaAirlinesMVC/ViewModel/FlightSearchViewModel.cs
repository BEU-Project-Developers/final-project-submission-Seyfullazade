using BetaAirlinesMVC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BetaAirlinesMVC.ViewModel
{
    public class FlightSearchViewModel
    {
        [Required]
        public string Location { get; set; }

        [Required]
        public int Travellers { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Departure { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Return { get; set; }

        public List<Flight> SearchResults { get; set; }
    }
}