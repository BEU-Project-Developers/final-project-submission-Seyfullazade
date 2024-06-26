﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BetaAirlinesMVC.Models
{
    [Table("BookedFlights")]
    public class BookedFlight
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Date Booked")]
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateBooked { get; set; }

        [DefaultValue(1)]
        public int Active { get; set; }


        // supply a virtual object to the class to link the foreign key. 
        // Datatype of the foreign key object goes to, then whatever you name the virtual object, name the ForeignKey the same exactly.
        [Required]
        [ForeignKey("BookedUserId")]
        public int UserId { get; set; }

        //[NotMapped]
        public virtual User BookedUserId { get; set; }

        [Required]
        [ForeignKey("Flight")]
        public int FlightId { get; set; }

        //[NotMapped]
        public virtual Flight Flight { get; set; }
        public int Price { get; set; }

    }
}