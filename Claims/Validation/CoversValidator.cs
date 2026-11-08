using Claims.DTO;
using Claims.Exceptions;
using System.Net;

namespace Claims.Validation;

public class CoversValidator
{
    public void ValidateCreate(CoverRequestDto item)
    {
        if (item.EndDate <= item.StartDate)
        {
            throw CustomExceptionBuilderDirector.WithoutId(
                "END_DATE_INVALID",
                "End date must be after start date.",
                HttpStatusCode.BadRequest);
        }

        if (item.EndDate > item.StartDate.AddYears(1))
        {
            throw CustomExceptionBuilderDirector.WithoutId(
                "INSURANCE_PERIOD_INVALID",
                "Total insurance period cannot exceed 1 year.", HttpStatusCode.BadRequest);
        }
    }

    public void ValidateDelete(bool containsClaims)
    {
        if (containsClaims)
        {
            throw CustomExceptionBuilderDirector.WithoutId(
                "COVER_CONTAINS_CLAIMS",
                "Cover has existing claims.", HttpStatusCode.BadRequest );
        }
    }
}