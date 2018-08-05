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
using ConcertApplication.Services;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace ConcertApplication.Controllers
{
    //[Authorize]
    public class ConcertsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _appEnvironment;
        private readonly IEmailSender _emailSender;

        public ConcertsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            IHostingEnvironment appEnvironment, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _appEnvironment = appEnvironment;
            _emailSender = emailSender;
        }
        public List<ConcertModel> GetConcerts()
        {
            return _context.Concerts.ToList();
        }
        public List<string> GetPlaces()
        {
           return  _context.Concerts.Select(c => c.Place).ToList();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Add()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Add(ConcertViewModel model)
        {
            if (ModelState.IsValid)
            {
                ConcertModel concert = await _context.Concerts.FirstOrDefaultAsync(u => u.Name == model.Name);

                if (concert == null)
                {
                    ConcertModel c = new ConcertModel(model);

                    if (model.Type == nameof(ClassicalConcertModel))
                    {
                        ClassicalConcertModel classicConcert = new ClassicalConcertModel(c);
                        classicConcert.VocalType = model.VocalType;
                        classicConcert.ClassicalConcertName = model.ClassicalConcertName;
                        classicConcert.Composer = model.Composer;

                        _context.ClassicalConcerts.Add(classicConcert);
                    }
                    else if (model.Type == nameof(OpenAirModel))
                    {
                        OpenAirModel openAir = new OpenAirModel(c);

                        openAir.DriveWay = model.DriveWay;
                        openAir.Headliner = model.Headliner;

                        _context.OpenAirs.Add(openAir);
                    }
                    else if (model.Type == nameof(PartyModel))
                    {
                        PartyModel party = new PartyModel(c);
                        party.AgeQualification = model.AgeQualification.Value;
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
        public async Task<IActionResult> Index(string concertType, string searchString, int page)
        {
            List<ConcertModel> concerts = await _context.Concerts.ToListAsync();

            if (!String.IsNullOrEmpty(searchString))
            {
                concerts = concerts.Where(s => s.Name.Contains(searchString)
                || s.Performer.Contains(searchString)).ToList();
            }
            if (!string.IsNullOrEmpty(concertType))
            {
                concerts = concerts.Where(x => x.Type == concertType).ToList();
            }

            return View(concerts);
        }

        public IActionResult SearchAndFilter()
        {
            return View();
        }
        
        public async Task<IActionResult> Details(int? id)
        {
            if (TempData["result"] != null)
            {
                ViewBag.Message = TempData["result"].ToString();
                TempData["result"] = null;
            }
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
            ConcertViewModel model = new ConcertViewModel(concert);
            var type = concert.GetType().Name;
            if (type == nameof(ClassicalConcertModel))
            {
                ClassicalConcertModel classicalConcert = (ClassicalConcertModel)concert;
                model.ClassicalConcertName = classicalConcert.ClassicalConcertName;
                model.VocalType = classicalConcert.VocalType;
                model.Composer = classicalConcert.Composer;
            }
            else if (type == nameof(OpenAirModel))
            {
                OpenAirModel openAir = (OpenAirModel)concert;
                model.DriveWay = openAir.DriveWay;
                model.Headliner = openAir.Headliner;
            }
            else
            {
                PartyModel party = (PartyModel)concert;
                model.AgeQualification = party.AgeQualification;
            }

            return View(model);
        }

        public bool ValidateBookingData(ConcertModel currentConcert, int? id, int amount)
        {
            if (amount < 1)
            {
                TempData["result"] = "Ticket amount must be a positive number equal or greater than 1.";
                return false;
            }
            else if (currentConcert.TicketsLeft == 0)
            {
                TempData["result"] = "No more tickets left";
                return false;
            }
            else if (currentConcert.TicketsLeft > 0 && currentConcert.TicketsLeft < amount)
            {
                TempData["result"] = "Only " + currentConcert.TicketsLeft + " tickets left.";
                return false;
            }
            return true;
        }

        public async Task<IActionResult> BookTicket(int? id, int amount)
        {
            ConcertModel concert = await _context.Concerts.SingleOrDefaultAsync(c => c.Id == id);

            if (concert == null)
            {
                return NotFound();
            }
            else
            {
                var currentConcert = await _context.Concerts
                .SingleOrDefaultAsync(m => m.Id == id);

                if (!ValidateBookingData(currentConcert, id, amount))
                {
                    return RedirectToAction("Details", new { id = id });
                }
                else
                {
                    string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var currentUser = await _userManager.GetUserAsync(HttpContext.User);

                    TicketModel ticket = new TicketModel
                    {
                        IdConcert = currentConcert.Id,
                        IdUser = userId,
                        Amount = amount
                    };

                    currentConcert.TicketsLeft -= amount;

                    await _context.Tickets.AddAsync(ticket);
                    await _context.SaveChangesAsync();

                    string emailAddress = /*currentUser.Email;
                    emailAddress =*/ "julia08091997@mail.ru";

                    _emailSender.SendBookingConfirmationAsync(_appEnvironment, emailAddress, amount, currentConcert);
                    
                    TempData["result"] = "Your ticket is booked. Email confirmation is send to your email.";
                    return RedirectToAction("Details", new { id = id });
                }
            }
        }
        
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ConcertModel concert = await _context.Concerts.SingleOrDefaultAsync(m => m.Id == id);
            if (concert == null)
            {
                return NotFound();
            }

            ConcertViewModel model = new ConcertViewModel(concert);

            model.Types = new List<SelectListItem> {
                    new SelectListItem {Text = "Classical concert", Value = "classical"},
                    new SelectListItem {Text = "Open air", Value = "open_air"},
                    new SelectListItem {Text = "Party", Value = "party"}
                };
            model.EditUrl = this.Url.Action("Edit", new { id = concert.Id });
            model.DeleteUrl = this.Url.Action("Delete", new { id = concert.Id });
            model.DetailsUrl = this.Url.Action("Details", new { id = concert.Id });

            if (concert.Type == nameof(ClassicalConcertModel))
            {
                ClassicalConcertModel currentClassic = await _context.ClassicalConcerts.SingleOrDefaultAsync(c => c.Id == id);
                model.ClassicalConcertName = currentClassic.ClassicalConcertName;
                model.Composer = currentClassic.Composer;
                model.VocalType = currentClassic.VocalType;
            }
            else if (concert.Type == nameof(OpenAirModel))
            {
                OpenAirModel currentOpenAir = await _context.OpenAirs.SingleOrDefaultAsync(c => c.Id == id);
                model.Headliner = currentOpenAir.Headliner;
                model.DriveWay = currentOpenAir.DriveWay;
            }
            else if (concert.Type == nameof(PartyModel))
            {
                PartyModel currentParty = await _context.Parties.SingleOrDefaultAsync(c => c.Id == id);
                model.AgeQualification = currentParty.AgeQualification;
            }
            return View(model);
        }
        
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ConcertViewModel model)
        {            
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ConcertModel concert = await _context.Concerts.SingleOrDefaultAsync(c => c.Id == id);
                    if (concert.Type == model.Type)
                    {
                        if (model.Type == nameof(ClassicalConcertModel))
                        {
                            ClassicalConcertModel currentClassic = await _context.ClassicalConcerts.SingleOrDefaultAsync(c => c.Id == id);
                            currentClassic.ClassicalConcertName = model.ClassicalConcertName;
                            currentClassic.Composer = model.Composer;
                            currentClassic.VocalType = model.VocalType;
                            _context.Update(currentClassic);
                        }
                        else if (model.Type == nameof(OpenAirModel))
                        {
                            OpenAirModel currentOpenAir = await _context.OpenAirs.SingleOrDefaultAsync(c => c.Id == id);
                            currentOpenAir.DriveWay = model.DriveWay;
                            currentOpenAir.Headliner = model.Headliner;
                            _context.Update(currentOpenAir);
                        }
                        else if (model.Type == nameof(PartyModel))
                        {
                            PartyModel currentParty = await _context.Parties.SingleOrDefaultAsync(c => c.Id == id);
                            currentParty.AgeQualification = (int)model.AgeQualification;
                            _context.Update(currentParty);
                        }
                    }
                    else
                    {
                        if (model.Type == nameof(ClassicalConcertModel))
                        {
                            ClassicalConcertModel newConcert = new ClassicalConcertModel(concert);
                            newConcert.ClassicalConcertName = model.ClassicalConcertName;
                            newConcert.Composer = model.Composer;
                            newConcert.VocalType = model.VocalType;
                            _context.Concerts.Add(newConcert);
                        }
                        else if (model.Type == nameof(OpenAirModel))
                        {
                            OpenAirModel newConcert = new OpenAirModel(concert);
                            newConcert.DriveWay = model.DriveWay;
                            newConcert.Headliner = model.Headliner;
                            _context.Concerts.Add(newConcert);
                        }
                        else if (model.Type == nameof(PartyModel))
                        {
                            PartyModel newConcert = new PartyModel(concert);
                            newConcert.AgeQualification = (int)model.AgeQualification;
                            _context.Concerts.Add(newConcert);
                        }
                        _context.Concerts.Remove(concert);
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConcertExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
        
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
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
        
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var concert = await _context.Concerts.SingleOrDefaultAsync(m => m.Id == id);
            _context.Concerts.Remove(concert);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ConcertExists(int id)
        {
            return _context.Concerts.Any(e => e.Id == id);
        }
    }
}
