using Claims.Auditing;
using Claims.Auditing.Models;
using Claims.DTO;
using Claims.Exceptions;
using Claims.Models;
using Claims.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;


namespace Claims.Services
{
    /// <summary>
    /// Provides business logic for managing insurance covers, including retrieving,
    /// creating, deleting, and calculating cover premiums.
    /// </summary>
    /// <remarks>
    /// The service interacts with the database through <see cref="ClaimsRepository"/>,
    /// validates cover operations using <see cref="CoversValidator"/>,
    /// calculates premiums through <see cref="CoversServiceUtils"/>,
    /// and enqueues audit events through <see cref="IAuditQueue"/>.
    ///
    /// Custom exceptions are used to communicate business rule violations
    /// and resource-not-found scenarios.
    /// </remarks>
    public class CoversService : ICoversService
    {
        private readonly ClaimsRepository _claimsRepository;
        private readonly IAuditQueue _auditQueue;
        private readonly CoversValidator _coversValidator;
        private readonly CoversServiceUtils _coversServiceUtils;

        /// <summary>
        /// Initializes a new instance of <see cref="CoversService"/>.
        /// </summary>
        /// <param name="claimsRepository">The repository used to access cover and claim data.</param>
        /// <param name="auditQueue">The queue used to enqueue cover audit events.</param>
        /// <param name="coversValidator">The validator responsible for validating cover operations.</param>
        /// <param name="coversServiceUtils">Provides utility methods for calculating cover premiums.</param>
        public CoversService(ClaimsRepository claimsRepository, IAuditQueue auditQueue, CoversValidator coversValidator, CoversServiceUtils coversServiceUtils)
        {
            _claimsRepository = claimsRepository;
            _auditQueue = auditQueue;
            _coversValidator = coversValidator;
            _coversServiceUtils = coversServiceUtils;
        }

        /// <summary>
        /// Retrieves all covers from the database.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="CoverResponseDto"/> objects representing
        /// the covers stored in the database.
        /// </returns>
        public async Task<IEnumerable<CoverResponseDto>> GetCoversAsync()
        {
            return await _claimsRepository.Covers
                .Select(cover => new CoverResponseDto
                {
                    Id = cover.CoverId,
                    StartDate = cover.StartDate,
                    EndDate = cover.EndDate,
                    Type = cover.Type,
                    Premium = cover.Premium
                })
                .ToListAsync();
        }


        /// <summary>
        /// Retrieves a cover using its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the cover.</param>
        /// <returns>
        /// The corresponding <see cref="CoverResponseDto"/> if the cover exists.
        /// </returns>
        /// <exception cref="CustomApiException">
        /// Thrown when no cover with the specified identifier exists.
        /// </exception>
        public async Task<CoverResponseDto?> GetCoverAsync(string id)
        {
            var cover = await _claimsRepository.Covers.SingleOrDefaultAsync(cover => cover.CoverId == id);

            if (cover is null)
                throw CustomExceptionBuilderDirector.WithId(
                    "COVER_NOT_FOUND",
                    "The requested cover could not be found.",
                    id,
                    HttpStatusCode.NotFound);

            return new CoverResponseDto
            {
                Id = cover.CoverId,
                StartDate = cover.StartDate,
                EndDate = cover.EndDate,
                Type = cover.Type,
                Premium = cover.Premium
            };
        }

        /// <summary>
        /// Creates a new insurance cover.
        /// </summary>
        /// <remarks>
        /// The method validates the incoming request, calculates the cover premium,
        /// and persists the new cover to the database.
        ///
        /// After successfully saving the cover, an audit event is enqueued
        /// to record the creation operation.
        /// </remarks>
        /// <param name="item">The request containing the cover details.</param>
        /// <returns>
        /// An <see cref="ApiSuccessResponseDto{T}"/> containing the newly created
        /// cover and a success message.
        /// </returns>
        /// <exception cref="CustomApiException">
        /// Thrown when the cover request fails validation.
        /// </exception>
        public async Task<ApiSuccessResponseDto<CoverResponseDto>> AddCoverAsync(CoverRequestDto item)
        {
            _coversValidator.ValidateCreate(item);

            var cover = new Cover
            {
                CoverId = Guid.NewGuid().ToString(),
                StartDate = item.StartDate,
                EndDate = item.EndDate,
                Type = item.Type,
                Premium = _coversServiceUtils.ComputePremium(item.StartDate, item.EndDate, item.Type)
            };

            _claimsRepository.Covers.Add(cover);

            await _claimsRepository.SaveChangesAsync();

            // Enqueue an audit event for asynchronous processing,
            // recording the creation of the cover ("POST") and its identifier.
            await _auditQueue.EnqueueAsync(new AuditEvent(
                AuditEntityType.Cover,
                cover.CoverId,
                "POST",
                DateTime.UtcNow));

            var coverResponse = new CoverResponseDto
            {
                Id = cover.CoverId,
                StartDate = cover.StartDate,
                EndDate = cover.EndDate,
                Type = cover.Type,
                Premium = cover.Premium
            };

            return new ApiSuccessResponseDto<CoverResponseDto>
            {
                ReturnCode = "COVER_CREATED",
                Details = "Cover was successfully created.",
                Data = coverResponse
            };
        }


        /// <summary>
        /// Deletes an existing insurance cover from the database.
        /// </summary>
        /// <remarks>
        /// Before deletion, the method checks whether the cover has any associated claims.
        /// The cover validator ensures that covers containing claims cannot be deleted.
        ///
        /// After successfully deleting the cover, an audit event is enqueued
        /// to record the deletion operation.
        /// </remarks>
        /// <param name="id">The unique identifier of the cover to delete.</param>
        /// <returns>
        /// True if the cover was successfully deleted; otherwise, false.
        /// </returns>
        /// <exception cref="CustomApiException">
        /// Thrown when the cover does not exist or cannot be deleted
        /// because it contains associated claims.
        /// </exception>
        public async Task<bool> DeleteCoverAsync(string id)
        {
            var cover = await _claimsRepository.Covers
                .SingleOrDefaultAsync(cover => cover.CoverId == id);

            if (cover is null)
            {
                throw CustomExceptionBuilderDirector.WithId(
                    "COVER_NOT_FOUND",
                    "The requested cover could not be found.",
                    id,
                    HttpStatusCode.NotFound);
            }

            var containsClaims = await _claimsRepository.Claims
                .AnyAsync(claim => claim.CoverId == id);

            _coversValidator.ValidateDelete(containsClaims);

            _claimsRepository.Covers.Remove(cover);

            var result = await _claimsRepository.SaveChangesAsync();

            if (result > 0)
            {
                // Enqueue an audit event for asynchronous processing,
                // recording the deletion of the cover ("DELETE") and its identifier.
                await _auditQueue.EnqueueAsync(new AuditEvent(
                    AuditEntityType.Cover,
                    id,
                    "DELETE",
                    DateTime.UtcNow));
            }

            return result > 0;

        }

        /// <summary>
        /// Calculates the premium for an insurance cover based on its duration and type.
        /// </summary>
        /// <remarks>
        /// Delegates the premium calculation to <see cref="CoversServiceUtils"/>.
        /// Keeping ComputePremium public for algorithm testing purposes.
        /// </remarks>
        /// <param name="startDate">The start date of the insurance cover.</param>
        /// <param name="endDate">The end date of the insurance cover.</param>
        /// <param name="coverType">The type of insurance cover.</param>
        /// <returns>The calculated insurance premium.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the supplied date range is invalid.
        /// </exception>
        public decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
        {
            return _coversServiceUtils.ComputePremium(startDate, endDate, coverType);
        }
    }
}