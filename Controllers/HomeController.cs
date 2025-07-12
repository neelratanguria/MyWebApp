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
            var redirectData = _context.T_CEF_QR_DETAILS.ToList();
            
            // Pass locations for the dropdown - show description but store code ID
            ViewBag.Locations = new SelectList(_context.T_CODE_MASTER.ToList(), "CodeId", "CodeDescription");
            
            // Create a dictionary for location lookup
            var locationLookup = _context.T_CODE_MASTER.ToDictionary(l => l.CodeId, l => l.CodeDescription);
            ViewBag.LocationLookup = locationLookup;
            
            return View(redirectData);
        }

        [HttpPost]
        public IActionResult AddItem(string title, string description, string forwardToUrl, string location)
        {
            if (!string.IsNullOrWhiteSpace(title) && 
                !string.IsNullOrWhiteSpace(description) && 
                !string.IsNullOrWhiteSpace(forwardToUrl) && 
                !string.IsNullOrWhiteSpace(location))
            {
                var item = new Item
                {
                    Title = title,
                    Description = description,
                    ForwardToUrl = forwardToUrl,
                    Location = location,
                    CreatedDateTime = DateTime.UtcNow,
                    CreatedBy = "System"
                };
                _context.T_CEF_QR_DETAILS.Add(item);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpGet("/{id:int}")]
        public IActionResult RedirectToUrl(int id)
        {
            var item = _context.T_CEF_QR_DETAILS.FirstOrDefault(i => i.Id == id);
            if (item == null || string.IsNullOrEmpty(item.ForwardToUrl))
                return NotFound();

            // Register user IP and log with item primary key
            var userIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            var log = new RedirectLog
            {
                QRId = item.Id,
                IpAddress = userIp,
                CreatedOn = DateTime.UtcNow
            };
            _context.T_CEF_QR_RedirectLogs.Add(log);
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
            var item = _context.T_CEF_QR_DETAILS.FirstOrDefault(i => i.Id == id);
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

        [HttpGet]
        public IActionResult QrCodePage(int id)
        {
            var item = _context.T_CEF_QR_DETAILS.FirstOrDefault(i => i.Id == id);
            if (item == null)
                return NotFound();

            // Build the full URL for the QR code using the item ID
            var fullUrl = BaseSiteUrl.TrimEnd('/') + "/" + item.Id;
            
            ViewBag.FullUrl = fullUrl;
            ViewBag.QrCodeUrl = Url.Action("QrCode", "Home", new { id = item.Id });
            
            return View(item);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult RedirectLogs()
        {
            var logs = _context.T_CEF_QR_RedirectLogs.OrderByDescending(l => l.CreatedOn).ToList();
            return View(logs);
        }
    }
}
