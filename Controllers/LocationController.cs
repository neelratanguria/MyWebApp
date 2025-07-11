using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyWebApp.Models;

namespace MyWebApp.Controllers
{
    public class LocationController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<LocationController> _logger;

        public LocationController(ILogger<LocationController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: Location
        public IActionResult Index()
        {
            var locations = _context.Locations.ToList();
            return View(locations);
        }
    }
}
