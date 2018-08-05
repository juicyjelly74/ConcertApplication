using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ConcertApplication.Models
{
    public class ConcertViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Select Type")]
        public string Type { get; set; }

        public List<SelectListItem> Types { set; get; }

        [Required(ErrorMessage = "Enter Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Enter Performer")]
        public string Performer { get; set; }

        [Required(ErrorMessage = "Enter Total Tickets Amount. It must be a positive number equal or greater 1.")]
        [Range(1, Int32.MaxValue)]
        public int TicketsAmount { get; set; }
        
        public int TicketsLeft { get; set; }

        [Required(ErrorMessage = "Enter ConcertDate")]
        public DateTime ConcertDate { get; set; }

        [Required(ErrorMessage = "Enter Place")]
        public string Place { get; set; }

        [Required(ErrorMessage = "Enter Price. It must be a positive number.")]
        [Range(0, Int32.MaxValue)]
        public int Price { get; set; }

        public string VocalType { get; set; }

        public string ClassicalConcertName { get; set; }

        public string Composer { get; set; }

        public string DriveWay { get; set; }

        public string Headliner { get; set; }

        [Range(0, 100)]
        public int? AgeQualification { get; set; }

        [Required(ErrorMessage = "Enter ticket amount. It should be positive number equal or greater 1")]
        [Range(0, Int32.MaxValue)]
        public int BookingTicketAmount { get; set; }


        public string EditUrl { get; set; }
        public string DetailsUrl { get; set; }
        public string DeleteUrl { get; set; }
        
        public ConcertViewModel() { }
        public ConcertViewModel(ConcertModel concert)
        {
            Id = concert.Id;
            Name = concert.Name;
            Performer = concert.Performer;
            TicketsAmount = concert.TicketsAmount;
            TicketsLeft = concert.TicketsLeft;
            ConcertDate = concert.Date;
            Place = concert.Place;
            Price = concert.Price;
            Type = concert.Type;
        }
    }
}