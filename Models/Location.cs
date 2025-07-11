using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApp.Models
{
    [Table("LOCATION")]
    public class Location
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("LOCATION_NAME")]
        [Required]
        [StringLength(200)]
        public string LocationName { get; set; } = string.Empty;
    }
}
