using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApp.Models
{
    [Table("T_CODE_MASTER")]
    public class Location
    {
        [Key]
        [Column("TCM_CODE_ID")]
        [StringLength(20)]
        public string CodeId { get; set; } = string.Empty;

        [Column("TCM_CODE_DES")]
        [StringLength(255)]
        public string? CodeDescription { get; set; }

        [Column("TCM_TYPE_ID")]
        [StringLength(20)]
        public string? TypeId { get; set; }
    }
}
