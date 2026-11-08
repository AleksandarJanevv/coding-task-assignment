using Claims.Auditing;
using Claims.DTO;
using Claims.Models;
using Claims.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Claims.Controllers
{
    /// <summary>
    /// Handles HTTP requests related to claim operations.
    /// </summary>
    /// <remarks>
    /// Delegates business logic to <see cref="IClaimsService"/> and returns
    /// the appropriate HTTP responses to API consumers.
    /// </remarks>
    [ApiController]
    [Route("claims")]
    public class ClaimsController : ControllerBase
    {
        private readonly IClaimsService _claimsService;

        /// <summary>
        /// Initializes a new instance of <see cref="ClaimsController"/>.
        /// </summary>
        /// <param name="claimsService">The service responsible for managing claims.</param>
        public ClaimsController(IClaimsService claimsService)
        {
            _claimsService = claimsService;
        }

        /// <summary>
        /// Retrieves all claims.
        /// </summary>
        /// <returns>A collection of claims with an HTTP 200 response.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClaimResponseDto>>> GetAsync()
        {
            var result = await _claimsService.GetClaimsAsync();
            return Ok(result);
        }

        /// <summary>
        /// Creates a new claim associated with an existing cover.
        /// </summary>
        /// <param name="claim">The request containing the claim details.</param>
        /// <returns>The created claim and a success response.</returns>
        [HttpPost]
        public async Task<ActionResult<ApiSuccessResponseDto<ClaimResponseDto>>> CreateAsync(ClaimRequestDto claim)
        {

            var result = await _claimsService.AddClaimAsync(claim);
            return Ok(result);
        }

        /// <summary>
        /// Deletes an existing claim.
        /// </summary>
        /// <param name="id">The unique identifier of the claim to delete.</param>
        /// <returns>An HTTP 204 response when the deletion succeeds.</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(string id)
        {
            var result = await _claimsService.DeleteClaimAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Retrieves a claim by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the claim.</param>
        /// <returns>The requested claim with an HTTP 200 response.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ClaimResponseDto>> GetAsync(string id)
        {
            await _claimsService.DeleteClaimAsync(id);
            return NoContent();
        }
    }
}
