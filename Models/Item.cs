using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApp.Models
{
    [Table("T_CEF_QR_DETAILS")]
    public class Item
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }
        
        [Column("Title")]
        [Required(ErrorMessage = "Title is required")]
        [StringLength(255)]
        public string Title { get; set; } = string.Empty;
        
        [Column("Description")]
        [Required(ErrorMessage = "Description is required")]
        [StringLength(255)]
        public string Description { get; set; } = string.Empty;
        
        [Column("Location")]
        [Required(ErrorMessage = "Location is required")]
        [StringLength(255)]
        public string Location { get; set; } = string.Empty;
        
        [Column("ForwardtoURL")]
        [Required(ErrorMessage = "Forward URL is required")]
        [StringLength(2048)]
        public string ForwardToUrl { get; set; } = string.Empty;
        
        [Column("CreatedOn")]
        public DateTime CreatedDateTime { get; set; }
        
        [Column("CreatedBy")]
        [StringLength(255)]
        public string? CreatedBy { get; set; }
    }
}
