using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ConcertApplication.Models
{
    public class Concert
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Performer { get; set; }
        public int TicketsAmount { get; set; }
        public int TicketsLeft { get; set; }
        public DateTime Date { get; set; }
        public string Place { get; set; }
        public double Price { get; set; }
        public string Type { get; set; }

        public Concert() { }
        public Concert(Concert c)
        {
            Name = c.Name;
            Performer = c.Performer;
            TicketsAmount = c.TicketsAmount;
            TicketsLeft = c.TicketsAmount;
            Date = c.Date;
            Place = c.Place;
            Price = c.Price;
        }

        public override string ToString()
        {
            string result = "\nConcert " + this.Name + " with info:\n";
            PropertyInfo[] properties = GetType().GetProperties();
            foreach (var item in properties)
            {
                result += item.Name + ": " + item.GetValue(this) + "\n";
            }
            return result;
        }
    }

    public class ClassicConcert : Concert
    {
        public string VocalType { get; set; }
        public string ClassicalConcertName { get; set; }
        public string Composer { get; set; }
        public ClassicConcert() : base() { }
        public ClassicConcert(Concert c) : base(c) { }
    }
    public class Party : Concert
    {
        public int AgeQualification { get; set; }
        public Party() : base() { }
        public Party(Concert c) : base(c) { }
    }
    public class OpenAir : Concert
    {
        public string DriveWay { get; set; }
        public string Headliner { get; set; }
        public OpenAir() : base() { }
        public OpenAir(Concert c) : base(c) { }
    }

    public class TicketOrder
    {
        public int Id { get; set; }
        public int TicketAmount { get; set; }
        public int IdConcert { get; set; }
        public string IdUser { get; set; }
    }


    public class ConcertModel
    {
        [Required(ErrorMessage = "Select Type")]
        public string Type { get; set; }
        
        [Required(ErrorMessage = "Enter Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Enter Performer")]
        public string Performer { get; set; }

        [Required(ErrorMessage = "Enter Total Tickets Amount. It must be a positive number.")]
        [Range(0, Int32.MaxValue)]
        public int TicketsAmount { get; set; }

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

    }


    public class TicketOrderModel
    {
        [Required(ErrorMessage = "Enter ticket amount")]
        public int TicketAmount { get; set; }
    }
}