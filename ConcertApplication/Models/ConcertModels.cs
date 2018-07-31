using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
        public string Discriminator { get; set; }
    }

    public class ClassicConcert : Concert
    {
        public string VocalType { get; set; }
        public string ClassicalConcertName { get; set; }
        public string Composer { get; set; }
    }
    public class Party : Concert
    {
        public int AgeQulification { get; set; }
    }
    public class OpenAir : Concert
    {
        public string DriveWay { get; set; }
        public string Headliner { get; set; }
    }

    public class Ticket
    {
        public int Id { get; set; }
        public int IdConcert { get; set; }
        public int IdUser { get; set; }
    }


    public class ConcertModel
    {
        public string Type { get; set; }
        
        public string Name { get; set; }
        
        public string Performer { get; set; }
        
        public int TicketsAmount { get; set; }

        //[Required]
        //public DateTime ConcertDate { get; set; }
        
        public string Place { get; set; }
        
        public int Price { get; set; }

        public string VocalType { get; set; }

        public string ClassicalConcertName { get; set; }

        public string Composer { get; set; }

        public string DriveWay { get; set; }

        public string Headliner { get; set; }

        public int AgeQulification { get; set; }

    }
}