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
        public void Update(ConcertViewModel model)
        {
            base.Update(model);
            DriveWay = model.DriveWay;
            Headliner = model.Headliner;
        }
    }
}