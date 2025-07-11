using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyWebApp.Models;
using QRCoder;
using System.Drawing;
using System.IO;
using System.Net;

namespace MyWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<HomeController> _logger;
        private const string BaseSiteUrl = "https://4809513c5614.ngrok-free.app"; // Change to your deployed base URL

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var redirectData = _context.Redirect_data.Include(r => r.Location).ToList();
            
            // Pass locations for the dropdown
            ViewBag.Locations = new SelectList(_context.Locations.ToList(), "Id", "LocationName");
            
            return View(redirectData);
        }

        [HttpPost]
        public IActionResult AddItem(string? title, string? description, string? forwardToUrl, string? createdByName, int locationId)
        {
            if (!string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(forwardToUrl) && locationId > 0)
            {
                var item = new Item
                {
                    Title = title,
                    Description = description ?? string.Empty,
                    ForwardToUrl = forwardToUrl,
                    OwnSiteUrl = string.Empty, // No longer needed for redirection
                    CreatedDateTime = DateTime.UtcNow,
                    CreatedByName = createdByName ?? string.Empty,
                    LocationId = locationId
                };
                _context.Redirect_data.Add(item);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpGet("/{id:int}")]
        public IActionResult RedirectToUrl(int id)
        {
            var item = _context.Redirect_data.FirstOrDefault(i => i.Id == id);
            if (item == null || string.IsNullOrEmpty(item.ForwardToUrl))
                return NotFound();

            // Register user IP and log with item primary key
            var userIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            var log = new RedirectLog
            {
                ItemId = item.Id,
                IpAddress = userIp,
                RedirectedAt = DateTime.UtcNow
            };
            _context.RedirectLogs.Add(log);
            _context.SaveChanges();

            _logger.LogInformation($"Redirected: {id} from IP: {userIp}");
            return RedirectPermanent(item.ForwardToUrl);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult QrCode(int id)
        {
            var item = _context.Redirect_data.FirstOrDefault(i => i.Id == id);
            if (item == null)
                return NotFound();

            // Build the full URL for the QR code using the item ID
            var fullUrl = BaseSiteUrl.TrimEnd('/') + "/" + item.Id;
            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrData = qrGenerator.CreateQrCode(fullUrl, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new PngByteQRCode(qrData);
                var qrCodeBytes = qrCode.GetGraphic(20);  // Size 20
                return File(qrCodeBytes, "image/png");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult RedirectLogs()
        {
            var logs = _context.RedirectLogs.OrderByDescending(l => l.RedirectedAt).ToList();
            return View(logs);
        }
    }
}
