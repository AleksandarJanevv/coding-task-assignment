using Claims.Models;

namespace Claims.DTO
{
    public class CoverRequestDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public CoverType Type { get; set; }
    }
}
