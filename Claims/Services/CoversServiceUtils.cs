using Claims.Exceptions;
using Claims.Models;
using System.Net;

namespace Claims.Services
{
    public class CoversServiceUtils
    {
        public decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
        {
            if (endDate <= startDate)
            {
                throw CustomExceptionBuilderDirector.WithoutId(
                        "END_DATE_INVALID",
                        "End date must be after start date.",
                        HttpStatusCode.BadRequest);
            }

            const decimal baseDayRate = 1250m;


            // Determine the base rate multiplier.

            // Can introduce Open/Closed Principle in case other cover types
            // with different multipliers get implemented in the future
            var multiplier = coverType switch
            {
                CoverType.Yacht => 1.1m,
                CoverType.PassengerShip => 1.2m,
                CoverType.Tanker => 1.5m,
                _ => 1.3m
            };

            var dailyRate = baseDayRate * multiplier;
            var insuranceDays = (decimal)(endDate - startDate).TotalDays;

            // Determine the number of days in each pricing period.
            var firstPeriodDays = Math.Min(insuranceDays, 30m);

            var secondPeriodDays = Math.Min(Math.Max(insuranceDays - 30m, 0m), 150m);

            var remainingDays = Math.Max(insuranceDays - 180m, 0m);

            // Determine the applicable discounts for each period depending the cover type.
            var secondPeriodDiscount = coverType == CoverType.Yacht ? 0.05m : 0.02m;

            var additionalDiscount = coverType == CoverType.Yacht ? 0.03m : 0.01m;

            // Calculate the premium for each period.
            var firstPeriodPremium = firstPeriodDays * dailyRate;

            var secondPeriodPremium = secondPeriodDays * dailyRate * (1 - secondPeriodDiscount);

            // The additional discount is applied ON TOP of the previous discount.
            var remainingPeriodPremium = remainingDays * dailyRate * (1 - secondPeriodDiscount - additionalDiscount);

            return firstPeriodPremium + secondPeriodPremium + remainingPeriodPremium;
        }
    }
}
/*{
    var multiplier = 1.3m;

    if (coverType == CoverType.Yacht)
        multiplier = 1.1m;

    if (coverType == CoverType.PassengerShip)
        multiplier = 1.2m;

    if (coverType == CoverType.Tanker)
        multiplier = 1.5m;

    var premiumPerDay = 1250m * multiplier;
    var insuranceLength = (endDate - startDate).TotalDays;
    var totalPremium = 0m;

    for (var i = 0; i < insuranceLength; i++)
    {
        if (i < 30)
            totalPremium += premiumPerDay;

        if (i < 180 && coverType == CoverType.Yacht)
            totalPremium += premiumPerDay - premiumPerDay * 0.05m;
        else if (i < 180)
            totalPremium += premiumPerDay - premiumPerDay * 0.02m;

        if (i < 365 && coverType != CoverType.Yacht)
            totalPremium += premiumPerDay - premiumPerDay * 0.03m;
        else if (i < 365)
            totalPremium += premiumPerDay - premiumPerDay * 0.08m;
    }

    return totalPremium;
}*/