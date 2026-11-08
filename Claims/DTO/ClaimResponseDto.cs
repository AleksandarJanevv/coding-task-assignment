using Claims.Models;

namespace Claims.DTO
{
    public class ClaimResponseDto
    {
        public required string CoverId { get; set; }

        public required DateTime Created { get; set; }

        public required string Name { get; set; }

        public required ClaimType Type { get; set; }

        public required decimal DamageCost { get; set; }
    }
}
