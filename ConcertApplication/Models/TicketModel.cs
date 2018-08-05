using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ConcertApplication.Models
{
    public class TicketModel
    {
        public int Id { get; set; }
        public int Amount { get; set; }
        public int IdConcert { get; set; }
        public string IdUser { get; set; }
    }
}