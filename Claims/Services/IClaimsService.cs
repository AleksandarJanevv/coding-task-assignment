using Claims.DTO;

namespace Claims.Services
{
    public interface IClaimsService
    {
        Task<IEnumerable<ClaimResponseDto>> GetClaimsAsync();

        Task<ApiSuccessResponseDto<ClaimResponseDto>> AddClaimAsync(ClaimRequestDto item);

        Task<ClaimResponseDto?> GetClaimAsync(string id);

        Task<bool> DeleteClaimAsync(string id);
    }
}
