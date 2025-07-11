using System;

namespace MyWebApp.Models
{
    public class RedirectLog
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string? IpAddress { get; set; }
        public DateTime RedirectedAt { get; set; }
    }
}
