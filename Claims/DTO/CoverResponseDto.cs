using Claims.Models;

namespace Claims.DTO
{
    public class CoverResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public CoverType Type { get; set; }
        public decimal Premium { get; set; }
    }
}
