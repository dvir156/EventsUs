﻿using System.Linq;
using System.Threading.Tasks;
using EventsUs.Data;
using EventsUs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventsUs.Areas.CalendarUser.Controllers
{
    [Authorize(Roles = "Admin,CalendarUser")]
    [Area("CalendarUser")]
    public class FindFriendsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FindFriendsController(ApplicationDbContext context)
        {
            _context = context;

        }

        public IActionResult Index(string searchString1, string searchString2, string searchString3)
        {


            var users = from e in _context.ApplicationUser
                         select e;
            if (!string.IsNullOrEmpty(searchString1))
            {
                users = users.Where(e => e.Email.Contains(searchString1));
            }
            if (!string.IsNullOrEmpty(searchString2))
            {
                users = users.Where(e => e.Age.Equals(searchString2));
            }
            if (!string.IsNullOrEmpty(searchString3))
            {
                users = users.Where(e => e.Country.Contains(searchString3));
            }
            return View(users);
        }

    }
}
