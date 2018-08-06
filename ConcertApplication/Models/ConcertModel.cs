using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ConcertApplication.Models
{
    public class ConcertModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Performer { get; set; }
        public int TicketsAmount { get; set; }
        public int TicketsLeft { get; set; }
        public DateTime Date { get; set; }
        public string Place { get; set; }
        public int Price { get; set; }
        public string Type { get; set; }

        public ConcertModel() { }
        public ConcertModel(ConcertModel c)
        {
            Name = c.Name;
            Performer = c.Performer;
            TicketsAmount = c.TicketsAmount;
            TicketsLeft = c.TicketsLeft;
            Date = c.Date;
            Place = c.Place;
            Price = c.Price;
        }
        public ConcertModel(ConcertViewModel model)
        {
            Name = model.Name;
            Performer = model.Performer;
            TicketsAmount = model.TicketsAmount;
            TicketsLeft = model.TicketsAmount;
            Date = model.ConcertDate;
            Place = model.Place;
            Price = model.Price;
            Type = model.Type;
        }
        public virtual void Update(ConcertViewModel model)
        {
            Name = model.Name;
            Performer = model.Performer;
            TicketsAmount = model.TicketsAmount;
            TicketsLeft = model.TicketsLeft;
            Date = model.ConcertDate;
            Place = model.Place;
            Price = model.Price;
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
}