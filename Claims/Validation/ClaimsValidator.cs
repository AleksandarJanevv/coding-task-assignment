using Claims.DTO;
using Claims.Exceptions;
using Claims.Models;
using System.Net;

namespace Claims.Validation;

public class ClaimsValidator
{
    public void ValidateCreate(ClaimRequestDto item, Cover cover, DateTime createdDate)
    {
        if (item.DamageCost > 100000)
        {
            throw CustomExceptionBuilderDirector.WithoutId(
                "DAMAGE_COST_EXCEEDED",
                "Damage cost cannot exceed 100,000.",
                HttpStatusCode.BadRequest);
        }

        if (createdDate < cover.StartDate || createdDate > cover.EndDate)
        {
            throw CustomExceptionBuilderDirector.WithoutId(
                "CLAIM_DATE_OUTSIDE_COVER_PERIOD",
                "Claim creation date must be within the related cover period.",
                HttpStatusCode.BadRequest);
        }
    }
}