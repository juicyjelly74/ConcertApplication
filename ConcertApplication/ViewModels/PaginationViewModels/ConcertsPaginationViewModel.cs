using ConcertApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcertApplication.ViewModels.Pagination
{
    public class ConcertsPaginationViewModel
    {
        public ConcertModel modelForHeader { get; set; }
        public IEnumerable<ConcertModel> ConcertModels { get; set; }
        public PageViewModel PageViewModel { get; set; }
    }
}
