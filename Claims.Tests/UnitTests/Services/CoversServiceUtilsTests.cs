using Claims.Exceptions;
using Claims.Models;
using Claims.Services;
using Xunit;

namespace Claims.Tests.UnitTests.Services
{
    /// <summary>
    /// Contains unit tests for the CoversServiceUtils class,
    /// specifically testing insurance premium calculations.
    /// </summary>
    public class CoversServiceUtilsTests
    {
        private readonly CoversServiceUtils _utils = new();

        /// <summary>
        /// Verifies that calculating a premium with an end date
        /// earlier than the start date throws a CustomApiException.
        /// </summary>
        /// <remarks>
        /// Expected error code: END_DATE_INVALID.
        /// </remarks>
        [Fact]
        public void ComputePremium_EndDateBeforeStartDate_ThrowsException()
        {
            var startDate = new DateTime(2026, 6, 1);
            var endDate = new DateTime(2026, 1, 1);

            var exception = Assert.Throws<CustomApiException>(() =>
                _utils.ComputePremium(startDate, endDate, CoverType.Yacht));

            Assert.Equal("END_DATE_INVALID", exception.ReturnCode);
        }

        /// <summary>
        /// Verifies that calculating a premium with identical start
        /// and end dates throws a CustomApiException.
        /// </summary>
        /// <remarks>
        /// Expected error code: END_DATE_INVALID.
        /// </remarks>
        [Fact]
        public void ComputePremium_EndDateEqualsStartDate_ThrowsException()
        {
            var date = new DateTime(2026, 1, 1);

            var exception = Assert.Throws<CustomApiException>(() =>
                _utils.ComputePremium(date, date, CoverType.Yacht));

            Assert.Equal("END_DATE_INVALID", exception.ReturnCode);
        }

        /// <summary>
        /// Verifies that a Yacht cover with a 30-day insurance period
        /// returns the expected premium without any discounts.
        /// </summary>
        /// <remarks>
        /// Calculation:
        /// Base daily rate: 1,250
        /// Yacht multiplier: 1.1
        /// Daily rate: 1,250 * 1.1 = 1,375
        ///
        /// First 30 days:
        /// 30 * 1,375 = 41,250
        ///
        /// Expected premium: 41,250.
        /// </remarks>
        [Fact]
        public void ComputePremium_Yacht30Days_ReturnsExpectedPremium()
        {
            var result = _utils.ComputePremium(
                new DateTime(2026, 1, 1),
                new DateTime(2026, 1, 31),
                CoverType.Yacht);

            Assert.Equal(41250m, result);
        }

        /// <summary>
        /// Verifies that a Yacht cover with a 31-day insurance period
        /// applies the 5% discount to the day following the first 30 days.
        /// </summary>
        /// <remarks>
        /// Calculation:
        /// Base daily rate: 1,250
        /// Yacht multiplier: 1.1
        /// Daily rate: 1,250 * 1.1 = 1,375
        ///
        /// First 30 days (no discount):
        /// 30 * 1,375 = 41,250
        ///
        /// Following 1 day (5% discount):
        /// 1 * 1,375 * (1 - 0.05) = 1,306.25
        ///
        /// Expected premium: 42,556.25.
        /// </remarks>
        [Fact]
        public void ComputePremium_Yacht31Days_ReturnsExpectedPremium()
        {
            var result = _utils.ComputePremium(
                new DateTime(2026, 1, 1),
                new DateTime(2026, 2, 1),
                CoverType.Yacht);

            Assert.Equal(42556.25m, result);
        }

        /// <summary>
        /// Verifies that a Yacht cover with a 181-day insurance period
        /// applies the correct discounts across all three pricing periods.
        /// </summary>
        /// <remarks>
        /// Calculation:
        /// Base daily rate: 1,250
        /// Yacht multiplier: 1.1
        /// Daily rate: 1,250 * 1.1 = 1,375
        ///
        /// First 30 days (no discount):
        /// 30 * 1,375 = 41,250
        ///
        /// Following 150 days (5% discount):
        /// 150 * 1,375 * (1 - 0.05) = 195,937.50
        ///
        /// Remaining 1 day (cumulative 8% discount):
        /// 1 * 1,375 * (1 - 0.05 - 0.03) = 1,265
        ///
        /// Expected premium: 238,452.50.
        /// </remarks>
        [Fact]
        public void ComputePremium_Yacht181Days_ReturnsExpectedPremium()
        {
            var result = _utils.ComputePremium(
                new DateTime(2026, 1, 1),
                new DateTime(2026, 7, 1),
                CoverType.Yacht);

            Assert.Equal(238452.50m, result);
        }
    }
}