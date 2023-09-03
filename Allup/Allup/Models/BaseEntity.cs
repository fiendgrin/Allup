using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Allup.Models
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        [Column(TypeName = "date")]
        public DateTime CreatedAt { get; set; }
        [StringLength(100)]
        public string CeatedBy { get; set; }
        [Column(TypeName = "date")]
        public DateTime? DeletedAt { get; set; }
        [StringLength(100)]
        public string? DeletedBy { get; set; }
        [Column(TypeName = "date")]
        public DateTime? UpdatedAt { get; set; }
        [StringLength(100)]
        public string? UpdatedBy { get; set; }
    }
}
