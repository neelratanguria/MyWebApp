namespace MyWebApp.Models
{
    public class Item
    {
        public int Id { get; set; } // Primary key for EF Core
        public string Title { get; set; }
        public string Description { get; set; }
        public string ForwardToUrl { get; set; }
        public string OwnSiteUrl { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string CreatedByName { get; set; }
    }
}
