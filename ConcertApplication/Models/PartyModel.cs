using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ConcertApplication.Models
{
    public class PartyModel : ConcertModel
    {
        public int AgeQualification { get; set; }
        public PartyModel() : base() { }
        public PartyModel(ConcertModel c) : base(c) {
            Type = nameof(PartyModel);
        }
        public void Update(ConcertViewModel model)
        {
            AgeQualification = (int)model.AgeQualification;
        }
    }
}