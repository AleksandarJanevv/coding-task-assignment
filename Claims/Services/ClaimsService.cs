using Claims.Auditing;
using Claims.Auditing.Models;
using Claims.Controllers;
using Claims.DTO;
using Claims.Exceptions;
using Claims.Models;
using Claims.Validation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Net;

namespace Claims.Services
{
    /// <summary>
    /// Provides business logic for managing claims, including retrieving,
    /// creating, and deleting claim records.
    /// </summary>
    /// <remarks>
    /// The service interacts with the database through <see cref="ClaimsRepository"/>,
    /// validates incoming claim requests using <see cref="ClaimsValidator"/>,
    /// and enqueues audit events through <see cref="IAuditQueue"/>.
    ///
    /// Custom exceptions are used to communicate business rule violations
    /// and resource-not-found scenarios.
    /// </remarks>
    public class ClaimsService : IClaimsService
    {
        private readonly ClaimsRepository _claimsRepository;
        private readonly IAuditQueue _auditQueue;
        private readonly ClaimsValidator _claimsValidator;


        /// <summary>
        /// Initializes a new instance of <see cref="ClaimsService"/>.
        /// </summary>
        /// <param name="claimsRepository">The repository used to access claim and cover data.</param>
        /// <param name="auditQueue">The queue used to enqueue claim audit events.</param>
        /// <param name="claimsValidator">The validator responsible for validating claim requests.</param>
        public ClaimsService(ClaimsRepository claimsRepository, IAuditQueue auditQueue, ClaimsValidator claimsValidator)
        {
            _claimsRepository = claimsRepository;
            _auditQueue = auditQueue;
            _claimsValidator = claimsValidator;
        }

        /// <summary>
        /// Retrieves all claims from the database.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="ClaimResponseDto"/> objects representing
        /// the claims stored in the database.
        /// </returns>
        public async Task<IEnumerable<ClaimResponseDto>> GetClaimsAsync()
        {
            return await _claimsRepository.Claims.Select(claim => new ClaimResponseDto
            {
                CoverId = claim.CoverId,
                Created = claim.Created,
                Name = claim.Name,
                Type = claim.Type,
                DamageCost = claim.DamageCost
            }).ToListAsync();
        }

        /// <summary>
        /// Creates a new claim associated with an existing cover.
        /// </summary>
        /// <remarks>
        /// The method first verifies that the requested cover exists,
        /// then validates the claim using <see cref="ClaimsValidator"/>.
        ///
        /// Once validated, the claim is persisted to the database and
        /// an audit event is enqueued to record the creation operation.
        /// </remarks>
        /// <param name="item">The request containing the claim details.</param>
        /// <returns>
        /// An <see cref="ApiSuccessResponseDto{T}"/> containing the newly created
        /// claim and a success message.
        /// </returns>
        /// <exception cref="CustomApiException">
        /// Thrown when the requested cover does not exist or when claim validation fails.
        /// </exception>
        public async Task<ApiSuccessResponseDto<ClaimResponseDto>> AddClaimAsync(ClaimRequestDto item)
        {
            var cover = await _claimsRepository.Covers
                .SingleOrDefaultAsync(cover => cover.CoverId == item.CoverId);

            if (cover == null)
            {
                throw CustomExceptionBuilderDirector.WithoutId(
                    "COVER_NOT_FOUND",
                    "The requested cover could not be found.",
                    HttpStatusCode.NotFound);
            }

            var createdDate = DateTime.UtcNow;

            _claimsValidator.ValidateCreate(item, cover, createdDate);

            var claim = new Claim
            {
                ClaimId = Guid.NewGuid().ToString(),
                Cover = cover,
                Created = createdDate,
                Name = item.Name,
                Type = item.Type,
                DamageCost = item.DamageCost
            };

            _claimsRepository.Claims.Add(claim);

            await _claimsRepository.SaveChangesAsync();

            // Enqueue an audit event for asynchronous processing,
            // recording the creation of the claim ("POST") and its identifier.
            await _auditQueue.EnqueueAsync(new AuditEvent(
                AuditEntityType.Claim,
                claim.ClaimId,
                "POST",
                DateTime.UtcNow));

            var claimResponse = new ClaimResponseDto
            {
                CoverId = claim.CoverId,
                Created = claim.Created,
                Name = claim.Name,
                Type = claim.Type,
                DamageCost = claim.DamageCost
            };

            return new ApiSuccessResponseDto<ClaimResponseDto>
            {
                ReturnCode = "CLAIM_CREATED",
                Details = "Claim was successfully created.",
                Data = claimResponse
            };
        }

        /// <summary>
        /// Retrieves a claim using its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the claim.</param>
        /// <returns>
        /// The corresponding <see cref="ClaimResponseDto"/> if the claim exists.
        /// </returns>
        /// <exception cref="CustomApiException">
        /// Thrown when no claim with the specified identifier exists.
        /// </exception>
        public async Task<ClaimResponseDto?> GetClaimAsync(string id)
        {
            var claim = await _claimsRepository.Claims
                .SingleOrDefaultAsync(claim => claim.ClaimId == id);

            if (claim is null)
            {
                throw CustomExceptionBuilderDirector.WithId(
                    "CLAIM_NOT_FOUND",
                    "The requested claim could not be found.",
                    id,
                    HttpStatusCode.NotFound);
            }

            return new ClaimResponseDto
            {
                CoverId = claim.CoverId,
                Created = claim.Created,
                Name = claim.Name,
                Type = claim.Type,
                DamageCost = claim.DamageCost
            };
        }

        /// <summary>
        /// Deletes an existing claim from the database.
        /// </summary>
        /// <remarks>
        /// After successfully deleting the claim, an audit event is enqueued
        /// to record the deletion operation.
        ///
        /// No audit event is generated if the database reports that no records
        /// were affected by the deletion.
        /// </remarks>
        /// <param name="id">The unique identifier of the claim to delete.</param>
        /// <returns>
        /// True if the claim was successfully deleted; otherwise, false.
        /// </returns>
        /// <exception cref="CustomApiException">
        /// Thrown when no claim with the specified identifier exists.
        /// </exception>
        public async Task<bool> DeleteClaimAsync(string id)
        {
            var claim = await _claimsRepository.Claims
                .SingleOrDefaultAsync(claim => claim.ClaimId == id);

            if (claim is null)
            {
                throw CustomExceptionBuilderDirector.WithId(
                    "CLAIM_NOT_FOUND",
                    "The requested claim could not be found.",
                    id, HttpStatusCode.NotFound);
            }

            _claimsRepository.Claims.Remove(claim);

            var result = await _claimsRepository.SaveChangesAsync();

            if (result > 0)
            {
                // Enqueue an audit event for asynchronous processing,
                // recording the deletion of the claim ("DELETE") and its identifier.
                await _auditQueue.EnqueueAsync(new AuditEvent(
                AuditEntityType.Claim,
                id,
                "DELETE",
                DateTime.UtcNow));
            }

            return result > 0;
        }
    }
}
