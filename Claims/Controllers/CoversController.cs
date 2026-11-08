using Claims.DTO;
using Claims.Models;
using Claims.Services;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers
{
    /// <summary>
    /// Handles HTTP requests related to insurance cover operations.
    /// </summary>
    /// <remarks>
    /// Delegates business logic to <see cref="ICoversService"/> and returns
    /// the appropriate HTTP responses to API consumers.
    /// </remarks>
    [ApiController]
    [Route("covers")]
    public class CoversController : ControllerBase
    {
        private readonly ICoversService _coversService;

        /// <summary>
        /// Initializes a new instance of <see cref="CoversController"/>.
        /// </summary>
        /// <param name="coversService">The service responsible for managing covers.</param>
        public CoversController(ICoversService coversService)
        {
            _coversService = coversService;
        }

        /// <summary>
        /// Retrieves all available insurance covers.
        /// </summary>
        /// <returns>A collection of covers with an HTTP 200 response.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CoverResponseDto>>> GetAsync()
        {
            var result = await _coversService.GetCoversAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves an insurance cover by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the cover.</param>
        /// <returns>The requested cover with an HTTP 200 response.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<CoverResponseDto>> GetAsync(string id)
        {
            var result = await _coversService.GetCoverAsync(id);

            return Ok(result);
        }

        /// <summary>
        /// Creates a new insurance cover.
        /// </summary>
        /// <param name="cover">The request containing the cover details.</param>
        /// <returns>The created cover and a success response.</returns>
        [HttpPost]
        public async Task<ActionResult<ApiSuccessResponseDto<CoverResponseDto>>> CreateAsync(CoverRequestDto cover)
        {
            var result = await _coversService.AddCoverAsync(cover);

            return Ok(result);
        }

        /// <summary>
        /// Deletes an existing insurance cover.
        /// </summary>
        /// <param name="id">The unique identifier of the cover to delete.</param>
        /// <returns>An HTTP 204 response when the deletion succeeds.</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(string id)
        {
            await _coversService.DeleteCoverAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Calculates the insurance premium based on the cover period and type.
        /// </summary>
        /// <param name="startDate">The start date of the cover.</param>
        /// <param name="endDate">The end date of the cover.</param>
        /// <param name="coverType">The type of insurance cover.</param>
        /// <returns>The calculated premium with an HTTP 200 response.</returns>
        [HttpPost("compute")]
        public ActionResult ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
        {

            var result = _coversService.ComputePremium(startDate, endDate, coverType);

            return Ok(result);
        }

    }
}