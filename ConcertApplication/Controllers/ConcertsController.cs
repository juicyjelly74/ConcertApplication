using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ConcertApplication.Data;
using ConcertApplication.Models;
using Microsoft.AspNetCore.Authorization;

namespace ConcertApplication.Controllers
{
    [Authorize]
    public class ConcertsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ConcertsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<Concert> GetConcerts()
        {
            return _context.Concerts;
        }

        [Authorize(Roles = "Admin")]
        // GET: Concerts/Create
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
                Concert concert = await _context.Concerts.FirstOrDefaultAsync(u => u.Performer == model.Performer);
                
                if (concert == null)
                {
                    concert = new Concert { /*Id = model.Id,*/
                        Performer = model.Performer, TicketsAmount = model.TicketsAmount,
                        TicketsLeft = model.TicketsAmount,
                        //Date = new DateTime(4562222),
                        Place = model.Place,
                        Price = model.Price
                    };

                    _context.Concerts.Add(concert);
                    await _context.SaveChangesAsync();


                    return View(model);
                }
                else

                ModelState.AddModelError("", "Некорректные данные");
            }
            return View(model);

        }

        // GET: Concerts
        public async Task<IActionResult> Index()
        {
            return View(await _context.Concerts.ToListAsync());
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

        // POST: Concerts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Performer,TicketsAmount,TicketsLeft,Date,Place,Price")] Concert concert)
        {
            if (ModelState.IsValid)
            {
                _context.Add(concert);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(concert);
        }
                    

        private bool ConcertExists(int id)
        {
            return _context.Concerts.Any(e => e.Id == id);
        }
    }
}
