﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventsUs.Data;
using EventsUs.Models;
using System.Globalization;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace EventsUs.Controllers
{
    public class EventsController : Controller
    {
        public readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Events
        [HttpPost]
        public IActionResult Index(string searchString/*, string groupBy*/)
        {
            var events = from e in _context.Event
                         select e;
            //if (groupBy == "City")
            //{
            //    var userNamesByID =
            //         from u in _context.Event
            //         group u by u.Location into g
            //         select new { Location = g.Key, count = g.Count(), g.First().Name };
            //    var group = new List<Event>();
            //    foreach (var t in userNamesByID)
            //    {
            //        group.Add(new Event()
            //        {
            //            Location = t.Location,
            //            numOfEvents = t.count,
            //        });
            //    }

            //    return View(group);
            //}
            if (!string.IsNullOrEmpty(searchString))
            {
                events = events.Where(e => e.Name.Contains(searchString));
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
        public async Task<IActionResult> Create([Bind("Id,Date,Name,Description,Location,PrivOrPubl")]
            Event @event)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@event);
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Date,Name,Description,Location,PrivOrPubl")]
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

        public bool EventExists(int id)
        {
            return _context.Event.Any(e => e.Id == id);
        }


        [HttpGet]
        public ViewResult getAllEventByDate(int day, int month, int year)
        {
            var @event = _context.Event.Where(e => e.Date.Day == day && e.Date.Month == month && e.Date.Year == year);
            return View(@event.ToList());
        }

        [HttpGet]
        public ViewResult getAllEventDetails(int day, int month, int year)
        {
            var @event = _context.Event.Where(e => e.Date.Day == day && e.Date.Month == month && e.Date.Year == year);
            return View(@event.ToList());

        }
    }
}
