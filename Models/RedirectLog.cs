using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApp.Models
{
    [Table("T_CEF_QR_RedirectLogs")]
    public class RedirectLog
    {
        [Key]
        [Column("Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Column("QRId")]
        public int QRId { get; set; }
        
        [Column("IpAddress")]
        [StringLength(45)]
        public string? IpAddress { get; set; }
        
        [Column("CreatedOn")]
        public DateTime CreatedOn { get; set; }
    }
}
