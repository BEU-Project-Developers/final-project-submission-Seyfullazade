using BetaAirlinesMVC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace BetaAirlinesMVC.ViewModel
{
    public class TicketViewModel
    {
        public BookedFlight BookedFlight { get; set; }
        public string DepartureCity { get; set; }
        public string ArrivalCity { get; set; }
        public string Username { get; set; }
    }
}