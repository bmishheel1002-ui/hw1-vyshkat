using VyshKat.Domain;

namespace VyshKat.Application;

/// <summary>
/// Applies the example technical-inspection rules.
/// </summary>
public sealed class TechnicalInspectionService : ITechnicalInspectionService
{
    // For this homework demo, items may use at most 10 kWh each day.
    private const decimal MaximumDailyEnergyConsumptionKwh = 10m;

    // Check whether the given item follows the inspection rules.
    public bool Approves(IInventoryItem item)
    {
        // Reject an energy user when its daily use is above the limit.
        if (item is IEnergyConsumer energyConsumer &&
            energyConsumer.DailyEnergyConsumptionKwh > MaximumDailyEnergyConsumptionKwh)
        {
            return false;
        }

        // Rental vehicles need a valid score; other inventory items pass this check.
        return item is IRentalVehicle rentalVehicle
            ? rentalVehicle.BeginnerFriendliness is >= 1 and <= 10
            : true;
    }
}
