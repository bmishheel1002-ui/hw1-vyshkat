namespace VyshKat.Domain;

/// <summary>
/// Describes an item that uses energy each day.
/// </summary>
public interface IEnergyConsumer
{
    /// <summary>
    /// Gets the daily energy use in kilowatt-hours.
    /// </summary>
    decimal DailyEnergyConsumptionKwh { get; }
}
