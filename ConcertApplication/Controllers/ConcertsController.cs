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

namespace ConcertApplication.Controllers
{
    //[Authorize]
    public class ConcertsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ConcertsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        
        public List<string> GetPlaces()
        {
            List<Concert> concerts = _context.Concerts.ToList();
            List<string> places = new List<string>();
            foreach (Concert c in concerts)
            {
                places.Add(c.Place);
            }
            return places;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Add()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Add(ConcertModel model)
        {
            if (ModelState.IsValid)
            {
                Concert concert = await _context.Concerts.FirstOrDefaultAsync(u => u.Name == model.Name);

                if (concert == null)
                {
                    Concert c = new Concert
                    {
                        Name = model.Name,
                        Performer = model.Performer,
                        TicketsAmount = model.TicketsAmount,
                        TicketsLeft = model.TicketsAmount,
                        Date = model.ConcertDate,
                        Place = model.Place,
                        Price = model.Price
                    };

                    if (model.Type == "classical")
                    {
                        ClassicConcert classicConcert = new ClassicConcert(c);
                        classicConcert.VocalType = model.VocalType;
                        classicConcert.ClassicalConcertName = model.ClassicalConcertName;
                        classicConcert.Composer = model.Composer;

                        _context.ClassicConcerts.Add(classicConcert);
                    }
                    else if (model.Type == "open_air")
                    {
                        OpenAir openAir = new OpenAir(c);

                        openAir.DriveWay = model.DriveWay;
                        openAir.Headliner = model.Headliner;

                        _context.OpenAirs.Add(openAir);
                    }
                    else
                    {
                        Party party = new Party(c);
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
            return View(concerts);
        }

        // GET: Concerts/Details/5
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

            return View(concert);
        }

        public async Task<IActionResult> BookTicket(int? id, int amount)
        {
            Concert concert = await _context.Concerts.SingleOrDefaultAsync(c => c.Id == id);

            if (concert == null)
            {
                return NotFound();
            }
            else
            {
                var currentConcert = await _context.Concerts
                .SingleOrDefaultAsync(m => m.Id == id);

                if (currentConcert.TicketsLeft == 0)
                {
                    TempData["result"] = "No more tickets left";
                    return RedirectToAction("Details", new { id = id });

                }

                else if (currentConcert.TicketsLeft > 0 && currentConcert.TicketsLeft < amount)
                {
                    TempData["result"] = "Only " + currentConcert.TicketsLeft + " tickets left.";
                    return RedirectToAction("Details", new { id = id });
                }
                else
                {
                    string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    var u = await _userManager.GetUserAsync(HttpContext.User);

                    TicketOrder ticket = new TicketOrder
                    {
                        IdConcert = currentConcert.Id,
                        IdUser = userId,
                        TicketAmount = amount
                    };

                    currentConcert.TicketsLeft -= amount;

                    await _context.Tickets.AddAsync(ticket);
                    await _context.SaveChangesAsync();

                    string emailAddress = u.Email;
                    Console.Write(emailAddress);

                    MailMessage mail = new MailMessage("fromm@gmail.com", emailAddress);
                    mail.Subject = "Ticket Booking";
                    mail.Body = "Your ticket booking for the concert " + currentConcert.ToString() + "was succesfully completed.";

                    SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);

                    smtpClient.Credentials = new System.Net.NetworkCredential()
                    {
                        UserName = "juicy.jelly74@gmail.com",
                        Password = "45gfgfvfvf"
                    };

                    smtpClient.EnableSsl = true;
                    ServicePointManager.ServerCertificateValidationCallback = delegate (object s,
                            System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                            System.Security.Cryptography.X509Certificates.X509Chain chain,
                            System.Net.Security.SslPolicyErrors sslPolicyErrors)
                    {
                        return true;
                    };

                    smtpClient.Send(mail);
                    TempData["result"] = "Your ticket is booked. Email confirmation is send to your email.";
                    return RedirectToAction("Details", new { id = id });
                }
            }
        }

        // GET: Concerts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var concert = await _context.Concerts.SingleOrDefaultAsync(m => m.Id == id);
            if (concert == null)
            {
                return NotFound();
            }

            ConcertModel model = new ConcertModel
            {
                Id = concert.Id,
                Name = concert.Name,
                Performer = concert.Performer,
                TicketsAmount = concert.TicketsAmount,
                TicketsLeft = concert.TicketsLeft,
                ConcertDate = concert.Date,
                Place = concert.Place,
                Price = concert.Price,
                Type = concert.Type,
                Types = new List<SelectListItem> {
                    new SelectListItem {Text = "Classical concert", Value = "classical"},
                    new SelectListItem {Text = "Open air", Value = "open_air"},
                    new SelectListItem {Text = "Party", Value = "party"}
                }
            };
            
            if (concert.Type == "classical")
            {
                ClassicConcert currentClassic = await _context.ClassicConcerts.SingleOrDefaultAsync(c => c.Id == id);
                model.ClassicalConcertName = currentClassic.ClassicalConcertName;
                model.Composer = currentClassic.Composer;
                model.VocalType = currentClassic.VocalType;
            }
            else if (concert.Type == "open_air")
            {
                OpenAir currentOpenAir = await _context.OpenAirs.SingleOrDefaultAsync(c => c.Id == id);
                model.Headliner = currentOpenAir.Headliner;
                model.DriveWay = currentOpenAir.DriveWay;
            }
            else if (concert.Type == "party")
            {
                Party currentParty = await _context.Parties.SingleOrDefaultAsync(c => c.Id == id);
                model.AgeQualification = currentParty.AgeQualification;
            }
            return View(model);
        }

        // POST: Concerts/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ConcertModel concert)
        {            
            if (id != concert.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    List<Concert> models = new List<Concert>();
                    ClassicConcert currentClassic = null;
                    OpenAir currentOpenAir = null;
                    Party currentParty = null;

                    if (concert.Type == "classical")
                    {
                        currentClassic = await _context.ClassicConcerts.SingleOrDefaultAsync(c => c.Id == id);
                        currentClassic.ClassicalConcertName = concert.ClassicalConcertName;
                        currentClassic.Composer = concert.Composer;
                        currentClassic.VocalType = concert.VocalType;
                    }
                    else if (concert.Type == "open_air")
                    {
                        currentOpenAir = await _context.OpenAirs.SingleOrDefaultAsync(c => c.Id == id);
                        currentOpenAir.DriveWay = concert.DriveWay;
                        currentOpenAir.Headliner = concert.Headliner;
                    }
                    else
                    {
                        currentParty = await _context.Parties.SingleOrDefaultAsync(c => c.Id == id);
                        currentParty.AgeQualification = (int)concert.AgeQualification;
                    }
                    models.Add(currentClassic);
                    models.Add(currentOpenAir);
                    models.Add(currentParty);
                    foreach (Concert c in models)
                    {
                        if (c != null)
                        {
                            c.Name = concert.Name;
                            c.Performer = concert.Performer;
                            c.TicketsAmount = concert.TicketsAmount;
                            c.TicketsLeft = concert.TicketsLeft;
                            c.Date = concert.ConcertDate;
                            c.Place = concert.Place;
                            c.Price = concert.Price;
                            _context.Update(c);
                        }
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConcertExists(concert.Id))
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
            return View(concert);
        }

        // GET: Concerts/Delete/5
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

        // POST: Concerts/Delete/5
        [HttpPost, ActionName("Delete")]
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
