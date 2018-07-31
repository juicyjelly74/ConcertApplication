using ConcertApplication.Data;
using ConcertApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcertApplication.Controllers
{
    [Authorize]
    public class ConcertsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private List<Concert> currentConcerts;

        public ConcertsController(ApplicationDbContext context)
        {
            _context = context;
            currentConcerts = context.Concerts.ToList();
        }

        [HttpGet]
        public IEnumerable<Concert> GetConcerts()
        {
            return _context.Concerts;
        }

        [Authorize(Roles = "Admin")]
        
        public IActionResult Add()
        {
            return View();
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(ConcertModel model)
        {
            if (ModelState.IsValid)
            {
                Concert concert = await _context.Concerts.FirstOrDefaultAsync(u => u.Name == model.Name);
                
                if (concert == null)
                {
                    if (model.Type == "classical")
                    {
                        ClassicConcert classicConcert = new ClassicConcert
                        { /*Id = model.Id,*/
                            Name = model.Name,
                            Performer = model.Performer,
                            TicketsAmount = model.TicketsAmount,
                            TicketsLeft = model.TicketsAmount,
                            //Date = new DateTime(4562222),
                            Place = model.Place,
                            Price = model.Price,
                            VocalType = model.VocalType,
                            ClassicalConcertName = model.ClassicalConcertName,
                            Composer = model.Composer
                        };
                        _context.ClassicConcerts.Add(classicConcert);
                    }
                    else if (model.Type == "open_air")
                    {
                        OpenAir openAir = new OpenAir
                        { /*Id = model.Id,*/
                            Name = model.Name,
                            Performer = model.Performer,
                            TicketsAmount = model.TicketsAmount,
                            TicketsLeft = model.TicketsAmount,
                            //Date = new DateTime(4562222),
                            Place = model.Place,
                            Price = model.Price,
                            DriveWay = model.DriveWay,
                            Headliner = model.Headliner
                        };
                        _context.OpenAirs.Add(openAir);
                    }
                    else
                    {
                        Party party = new Party
                        { /*Id = model.Id,*/
                            Name = model.Name,
                            Performer = model.Performer,
                            TicketsAmount = model.TicketsAmount,
                            TicketsLeft = model.TicketsAmount,
                            //Date = new DateTime(4562222),
                            Place = model.Place,
                            Price = model.Price,
                            AgeQulification = model.AgeQulification
                        };
                        _context.Parties.Add(party);
                    }

                        await _context.SaveChangesAsync();


                    return RedirectToAction("Index", "Concerts");
                }
                else

                ModelState.AddModelError("", "Некорректные данные");
            }
            return View(model);

        }

        // GET: Concerts
        public async Task<IActionResult> Index(string concertType, string searchString)
        {
            List<Concert> concerts = await _context.Concerts.ToListAsync();

            if (!String.IsNullOrEmpty(searchString))
            {
                concerts = concerts.Where(s => s.Name.Contains(searchString)
                || s.Performer.Contains(searchString)).ToList();
            }
            if (!string.IsNullOrEmpty(concertType))
            {
                concerts = concerts.Where(x => x.Type == concertType).ToList();
            }
            currentConcerts = concerts;
            return View(currentConcerts);
        }

        // GET: Concerts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var concert = await _context.Concerts
                .SingleOrDefaultAsync(m => m.Id == id);
            if (concert == null)
            {
                return NotFound();
            }

            return View(concert);
        }

        [Authorize(Roles = "Admin")]
        // GET: Concerts/Create
        public IActionResult Create()
        {
            return View();
        }
                           

        private bool ConcertExists(int id)
        {
            return _context.Concerts.Any(e => e.Id == id);
        }
    }
}
