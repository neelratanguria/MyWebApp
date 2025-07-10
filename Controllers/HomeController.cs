using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
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
            var redirectData = _context.Redirect_data.ToList();
            return View(redirectData);
        }

        [HttpPost]
        public IActionResult AddItem(string title, string description, string forwardToUrl, string createdByName)
        {
            if (!string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(forwardToUrl))
            {
                // Generate a unique 6-character endpoint
                string endpoint;
                do
                {
                    endpoint = Guid.NewGuid().ToString("N").Substring(0, 6);
                } while (_context.Redirect_data.Any(i => i.OwnSiteUrl == endpoint));

                var item = new Item
                {
                    Title = title,
                    Description = description,
                    ForwardToUrl = forwardToUrl,
                    OwnSiteUrl = endpoint,
                    CreatedDateTime = DateTime.UtcNow,
                    CreatedByName = createdByName
                };
                _context.Redirect_data.Add(item);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpGet("/{endpoint}")]
        public IActionResult RedirectToUrl(string endpoint)
        {
            var item = _context.Redirect_data.FirstOrDefault(i => i.OwnSiteUrl == endpoint);
            if (item == null)
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

            _logger.LogInformation($"Redirected: {endpoint} from IP: {userIp}");
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

            // Build the full URL for the QR code
            var fullUrl = BaseSiteUrl.TrimEnd('/') + "/" + item.OwnSiteUrl;
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
