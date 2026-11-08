using Claims.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace Claims.DTO
{
    public class ClaimRequestDto
    {

        public required string CoverId { get; set; }

        public required DateTime Created { get; set; }

        public required string Name { get; set; }

        public required ClaimType Type { get; set; }

        public required decimal DamageCost { get; set; }
    }
}
