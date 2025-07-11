using System;
using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Models
{
    public class Item
    {
        public int Id { get; set; } // Primary key for EF Core
        
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Forward URL is required")]
        public string ForwardToUrl { get; set; } = string.Empty;
        
        public DateTime CreatedDateTime { get; set; }
        
        // Foreign key for Location - now mandatory
        [Required(ErrorMessage = "Location is required")]
        public int LocationId { get; set; }
        
        // Navigation property
        public Location? Location { get; set; }
    }
}
