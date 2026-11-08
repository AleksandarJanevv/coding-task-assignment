
using Claims.DTO;
using Claims.Exceptions;
using Claims.Models;
using Claims.Validation;
using Xunit;

namespace Claims.Tests.UnitTests.Validation;

public class CoversValidatorTests
{
    private readonly CoversValidator _validator;

    public CoversValidatorTests()
    {
        _validator = new CoversValidator();
    }

    private static CoverRequestDto CreateCover(DateTime startDate, DateTime endDate)
    {
        return new CoverRequestDto
        {
            StartDate = startDate,
            EndDate = endDate,
            Type = CoverType.Yacht
        };
    }

/* Validate that valid cover does not throw an exception*/
    [Fact]
    public void ValidateCreate_ValidCover_DoesNotThrow()
    {
        // Arrange
        var cover = CreateCover(new DateTime(2026, 1, 1), new DateTime(2026, 6, 1));

        // Act & Assert
        var exception = Record.Exception(() =>
            _validator.ValidateCreate(cover));

        Assert.Null(exception);
    }

/* Validate that exception is thrown when end date is before start date of cover*/
    [Fact]
    public void ValidateCreate_EndDateBeforeStartDate_ThrowsException()
    {
        // Arrange
        var cover = CreateCover(new DateTime(2026, 6, 1), new DateTime(2026, 1, 1));

        // Act
        var exception = Assert.Throws<CustomApiException>(() =>
            _validator.ValidateCreate(cover));

        // Assert
        Assert.Equal("END_DATE_INVALID", exception.ReturnCode);
    }


/*Validate that when end date is equal to start date exception is thrown*/
    [Fact]
    public void ValidateCreate_EndDateEqualsStartDate_ThrowsException()
    {
        // Arrange
        var date = new DateTime(2026, 1, 1);
        var cover = CreateCover(date, date);

        // Act
        var exception = Assert.Throws<CustomApiException>(() =>
            _validator.ValidateCreate(cover));

        // Assert
        Assert.Equal("END_DATE_INVALID", exception.ReturnCode);
    }

/*Validate that when insurance period is more than one year exception is thrown*/
    [Fact]
    public void ValidateCreate_InsurancePeriodExceedsOneYear_ThrowsException()
    {
        // Arrange
        var cover = CreateCover(new DateTime(2026, 1, 1), new DateTime(2027, 1, 2));

        // Act
        var exception = Assert.Throws<CustomApiException>(() =>
            _validator.ValidateCreate(cover));

        // Assert
        Assert.Equal("INSURANCE_PERIOD_INVALID", exception.ReturnCode);
    }

    /*Validate that when insurance period is exactly one year no exception is thrown*/
    [Fact]
    public void ValidateCreate_InsurancePeriodExactlyOneYear_DoesNotThrow()
    {
        // Arrange
        var cover = CreateCover(new DateTime(2026, 1, 1), new DateTime(2027, 1, 1));

        // Act & Assert
        var exception = Record.Exception(() =>
            _validator.ValidateCreate(cover));

        Assert.Null(exception);
    }

    /*Validate that when cover contains claims, it does not get deleted and exception is thrown*/
    [Fact]
    public void ValidateDelete_CoverContainsClaims_ThrowsException()
    {
        // Arrange
        var containsClaims = true;

        // Act
        var exception = Assert.Throws<CustomApiException>(() =>
            _validator.ValidateDelete(containsClaims));

        // Assert
        Assert.Equal("COVER_CONTAINS_CLAIMS", exception.ReturnCode);
    }

    /*Validate when cover does not contain claims, delete does not throw an exception */
    [Fact]
    public void ValidateDelete_CoverDoesNotContainClaims_DoesNotThrow()
    {
        // Arrange
        var containsClaims = false;

        // Act & Assert
        var exception = Record.Exception(() =>
            _validator.ValidateDelete(containsClaims));

        Assert.Null(exception);
    }
}
