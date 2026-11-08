using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Claims.Models
{
    public class Cover
    {
        [Key]
        public string CoverId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public CoverType Type { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Premium { get; set; }

        public ICollection<Claim> Claims { get; set; } = new List<Claim>();
    }

}