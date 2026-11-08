using Claims.DTO;
using Claims.Exceptions;
using Claims.Models;
using Claims.Validation;
using Xunit;

namespace Claims.Tests.UnitTests.Validation;

public class ClaimsValidatorTests
{

    private readonly ClaimsValidator _validator;

    public ClaimsValidatorTests()
    {
        _validator = new ClaimsValidator();
    }


    /*Create a cover*/
    private static Cover CreateCover()
    {
        return new Cover
        {
            CoverId = Guid.NewGuid().ToString(),
            StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2026, 12, 31),
            Type = CoverType.Yacht,
            Premium = 1000
        };
    }
    
    /*Create a claim to attach to a cover*/
    private static ClaimRequestDto CreateClaim(decimal damageCost, string coverId, DateTime created)
    {
        return new ClaimRequestDto
        {
            CoverId = coverId,
            Created = created,
            Name = "Test claim",
            Type = ClaimType.Collision,
            DamageCost = damageCost
        };
    }

    /*Validate that creating a claim does not throw an exception when input for damage cost is 50k*/
    [Fact]
    public void ValidateCreate_ValidClaim_DoesNotThrow()
    {
        // Arrange
        var cover = CreateCover();
        var createdDate = new DateTime(2026, 6, 15);
        var claim = CreateClaim(50000, cover.CoverId, createdDate);

        // Act & Assert
        var exception = Record.Exception(() =>
            _validator.ValidateCreate(claim, cover, createdDate));

        Assert.Null(exception);
    }

    /*Validate that creating a claim does not throw an exception when input for damage cost equal to 100000k*/

    [Fact]
    public void ValidateCreate_DamageCostEquals100000_DoesNotThrow()
    {
        // Arrange
        var cover = CreateCover();
        var createdDate = new DateTime(2026, 6, 15);
        var claim = CreateClaim(100000, cover.CoverId, createdDate);

        // Act & Assert
        var exception = Record.Exception(() =>
            _validator.ValidateCreate(claim, cover, createdDate));

        Assert.Null(exception);
    }

/*     Validate that creating a claim does not throw an exception when input for damage cost more than 100000k */    
    [Fact]
    public void ValidateCreate_DamageCostExceeds100000_ThrowsException()
    {
        // Arrange
        var cover = CreateCover();
        var createdDate = new DateTime(2026, 6, 15);
        var claim = CreateClaim(100001, cover.CoverId, createdDate);

        // Act
        var exception = Assert.Throws<CustomApiException>(() =>
            _validator.ValidateCreate(claim, cover, createdDate));

        // Assert
        Assert.Equal("DAMAGE_COST_EXCEEDED", exception.ReturnCode);
    }

/* Validate that creating a claim throws an exception when the created date of claim is before the 
 * start date of the cover*/
    [Fact]
    public void ValidateCreate_CreatedBeforeCoverStarts_ThrowsException()
    {
        // Arrange
        var cover = CreateCover();
        var createdDate = new DateTime(2025, 12, 31);
        var claim = CreateClaim(50000, cover.CoverId, createdDate);

        // Act
        var exception = Assert.Throws<CustomApiException>(() =>
            _validator.ValidateCreate(claim, cover, createdDate));

        // Assert
        Assert.Equal("CLAIM_DATE_OUTSIDE_COVER_PERIOD", exception.ReturnCode);
    }

    /* Validate that creating a claim throws an exception when the created date of claim is after the 
     * start date of the cover*/
    [Fact]
    public void ValidateCreate_CreatedAfterCoverEnds_ThrowsException()
    {
        // Arrange
        var cover = CreateCover();
        var createdDate = new DateTime(2027, 1, 1);
        var claim = CreateClaim(50000, cover.CoverId, createdDate);

        // Act
        var exception = Assert.Throws<CustomApiException>(() =>
            _validator.ValidateCreate(claim, cover, createdDate));

        // Assert
        Assert.Equal("CLAIM_DATE_OUTSIDE_COVER_PERIOD", exception.ReturnCode);
    }
}