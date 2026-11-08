using Claims.DTO;
using Claims.Models;

namespace Claims.Services
{
    public interface ICoversService
    {
        decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType);
        Task<IEnumerable<CoverResponseDto>> GetCoversAsync();

        Task<CoverResponseDto?> GetCoverAsync(string id);

        Task<ApiSuccessResponseDto<CoverResponseDto>> AddCoverAsync(CoverRequestDto item);
        Task<bool> DeleteCoverAsync(string id);
    }
}
