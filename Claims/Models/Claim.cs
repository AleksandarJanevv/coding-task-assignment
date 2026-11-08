using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Claims.Models
{
    public class Claim
    {
        [Key]
        public string ClaimId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string CoverId { get; set; } = string.Empty;

        [ForeignKey(nameof(CoverId))]
        public Cover Cover { get; set; } = null!;

        [Required]
        public DateTime Created { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public ClaimType Type { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DamageCost { get; set; }
    }

}
