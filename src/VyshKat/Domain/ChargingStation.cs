namespace VyshKat.Domain;

/// <summary>
/// Represents a charging station tracked in inventory.
/// </summary>
public sealed class ChargingStation : IInventoryItem, IEnergyConsumer
{
    // Create the station using its inventory number and daily energy use.
    public ChargingStation(string inventoryNumber, decimal dailyEnergyConsumptionKwh)
    {
        // The inventory number must have a value.
        if (string.IsNullOrWhiteSpace(inventoryNumber))
        {
            throw new ArgumentException("Inventory number cannot be empty.", nameof(inventoryNumber));
        }

        // Energy use cannot be less than zero.
        if (dailyEnergyConsumptionKwh < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(dailyEnergyConsumptionKwh),
                dailyEnergyConsumptionKwh,
                "Energy consumption cannot be negative.");
        }

        // Save both checked values.
        InventoryNumber = inventoryNumber;
        DailyEnergyConsumptionKwh = dailyEnergyConsumptionKwh;
    }

    // Return the display name of this item.
    public string Name => "Charging station";

    // Return the station's inventory identifier.
    public string InventoryNumber { get; }

    // Return the station's daily energy use.
    public decimal DailyEnergyConsumptionKwh { get; }
}
