using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ConcertApplication.Models
{
    public class OpenAirModel : ConcertModel
    {
        public string DriveWay { get; set; }
        public string Headliner { get; set; }
        public OpenAirModel() : base() { }
        public OpenAirModel(ConcertModel c) : base(c)
        {
            Type = nameof(OpenAirModel);
        }
    }
}