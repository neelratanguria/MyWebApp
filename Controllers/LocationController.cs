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
            try
            {
                var locations = _context.T_CODE_MASTER.ToList();
                _logger.LogInformation($"Found {locations.Count} locations in T_CODE_MASTER");
                
                // Log each location for debugging
                foreach (var location in locations)
                {
                    _logger.LogInformation($"Location: {location.CodeId} - {location.CodeDescription} - {location.TypeId}");
                }
                
                return View(locations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving locations from T_CODE_MASTER");
                return View(new List<Location>());
            }
        }

        // GET: Location/Test - Test database connection
        public IActionResult Test()
        {
            try
            {
                // Test basic connection
                var connectionState = _context.Database.CanConnect();
                ViewBag.ConnectionStatus = connectionState ? "Connected" : "Not Connected";
                
                // Test if table exists by trying to query it
                var locations = _context.T_CODE_MASTER.ToList();
                ViewBag.LocationCount = locations.Count;
                ViewBag.Locations = locations;
                
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }
    }
}
