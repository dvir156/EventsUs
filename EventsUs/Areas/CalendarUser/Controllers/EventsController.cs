﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventsUs.Data;
using EventsUs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventsUs.Areas.CalendarUser.Controllers
{
    [Area("CalendarUser")]
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Events
        public async Task<IActionResult> Index(string searchString1, string searchString2, string searchString3,string gBy,string jBy)
        {
            if (gBy == "Location")
            {
                var EventByID =
                   from u in _context.Event
                   group u by u.Location into g
                   select new { Location = g.Key, count = g.Count(), g.First().Name };
                var group = new List<Event>();
                foreach (var t in EventByID)
                {
                    group.Add(new Event()
                    {
                        Name = "Event Counter:" + t.count.ToString(),
                        Location = t.Location
                    });
                }

                return View(group);
            }
            if (jBy == "Admin")
            {
                var join =
                from u in _context.Event

                join p in _context.Users on u.EventAdminId equals p.UserName

                select new { u.Name, u.Location, p.UserName,u.Date,u.Description };

                var UserList = new List<Event>();
                foreach (var t in join)
                {
                    UserList.Add(new Event()
                    {
                        Name = t.Name,
                        Location = t.Location,
                        Date = t.Date,
                        Description = t.Description

                    });
                }
                return View(UserList);
            }
            if (jBy == "Place")
            {
                var join =
                from u in _context.Event

                join p in _context.ApplicationUser on u.Location equals p.Country

                select new { u.Name, u.Location, p.UserName, u.Date, u.Description };

                var UserList = new List<Event>();
                foreach (var t in join)
                {
                    UserList.Add(new Event()
                    {
                        Name = t.Name,
                        Location = t.Location,
                        Date = t.Date,
                        Description = t.Description

                    });
                }
                return View(UserList);
            }
            var events = from e in _context.Event
                            select e;
            if(!string.IsNullOrEmpty(searchString1))
            {
                events = events.Where(e => e.Name.Contains(searchString1));
            }
            if (!string.IsNullOrEmpty(searchString2))
            {
                events = events.Where(e => e.Location.Contains(searchString2));
            }
            if (!string.IsNullOrEmpty(searchString3))
            {
                events = events.Where(e => e.Description.Contains(searchString3));
            }
            return View(events);
            //return View(await _context.Event.ToListAsync());
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Date,Name,Description,Location,YoutubeId,PublicPrivate")]
            Event @event)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@event);
                @event.EventAdminId = HttpContext.User.Identity.Name;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(@event);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Date,Name,Description,Location,YoutubeId,PublicPrivate")]
            Event @event)
        {
            if (id != @event.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.Id))
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

            return View(@event);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Event.FindAsync(id);
            _context.Event.Remove(@event);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Event.Any(e => e.Id == id);
        }


        [HttpGet]
        public ViewResult getAllEventByDate(int day, int month, int year, string username)
        {
            var @event = _context.Event.Where(e => (e.Date.Day == day && e.Date.Month == month && e.Date.Year == year) && (e.PublicPrivate == true||e.EventAdminId.Equals(username)));
            return View(@event.ToList());
        }

        [HttpGet]
        public ViewResult getAllEventDetails(int day, int month, int year,string username){
            var @event = _context.Event.Where(e => e.Date.Day == day && e.Date.Month == month && e.Date.Year == year&& (e.EventAdminId.Equals(username) || e.PublicPrivate == true)).Take(4);
            return View(@event.ToList());

        }
    }
}
