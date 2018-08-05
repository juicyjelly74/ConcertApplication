using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ConcertApplication.Models
{
    public class ClassicalConcertModel : ConcertModel
    {
        public string VocalType { get; set; }
        public string ClassicalConcertName { get; set; }
        public string Composer { get; set; }
        public ClassicalConcertModel() : base() { }
        public ClassicalConcertModel(ConcertModel c) : base(c)
        {
            Type = nameof(ClassicalConcertModel);
        }
    }
}