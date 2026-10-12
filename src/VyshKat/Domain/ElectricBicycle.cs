namespace VyshKat.Domain;

/// <summary>
/// Represents a rental bicycle that uses energy.
/// </summary>
public sealed class ElectricBicycle : RentalVehicle, IEnergyConsumer
{
    // Create the vehicle and pass its energy use as an extra value.
    public ElectricBicycle(
        string inventoryNumber,
        int beginnerFriendliness,
        decimal dailyEnergyConsumptionKwh)
        : base("Electric bicycle", inventoryNumber, beginnerFriendliness)
    {
        // Energy use cannot be less than zero.
        if (dailyEnergyConsumptionKwh < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(dailyEnergyConsumptionKwh),
                dailyEnergyConsumptionKwh,
                "Energy consumption cannot be negative.");
        }

        // Save the checked energy value.
        DailyEnergyConsumptionKwh = dailyEnergyConsumptionKwh;
    }

    // The amount of energy used in one day.
    public decimal DailyEnergyConsumptionKwh { get; }
}
