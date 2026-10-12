using VyshKat.Domain;

namespace VyshKat.Application;

/// <summary>
/// Runs fleet actions and prepares fleet reports.
/// </summary>
public sealed class FleetService(
    IFleetRepository fleetRepository,
    ITechnicalInspectionService technicalInspectionService)
{
    // A score of 6 or more is suitable for beginners.
    private const int BeginnerFriendlinessThreshold = 6;

    /// <summary>
    /// Inspects an item and saves it when it passes and its number is new.
    /// </summary>
    public bool TryAddToFleet(IInventoryItem item)
    {
        // Stop early if the caller passed no item.
        ArgumentNullException.ThrowIfNull(item);

        // A duplicate number or a failed inspection means the item is rejected.
        if (fleetRepository.ContainsInventoryNumber(item.InventoryNumber) ||
            !technicalInspectionService.Approves(item))
        {
            return false;
        }

        // Save an approved item and report success.
        fleetRepository.Add(item);
        return true;
    }

    /// <summary>
    /// Gets the number of saved items.
    /// </summary>
    public int GetItemCount() => fleetRepository.GetAll().Count;

    /// <summary>
    /// Gets the total daily energy use of saved items.
    /// </summary>
    public decimal GetDailyEnergyConsumptionKwh() =>
        fleetRepository
            .GetAll()
            .OfType<IEnergyConsumer>()
            .Sum(item => item.DailyEnergyConsumptionKwh);

    /// <summary>
    /// Gets rental vehicles with a beginner score of at least six.
    /// </summary>
    public IReadOnlyList<IRentalVehicle> GetVehiclesForBeginners() =>
        fleetRepository
            .GetAll()
            .OfType<IRentalVehicle>()
            .Where(vehicle => vehicle.BeginnerFriendliness >= BeginnerFriendlinessThreshold)
            .ToList();

    /// <summary>
    /// Gets every saved item.
    /// </summary>
    public IReadOnlyList<IInventoryItem> GetInventoryItems() => fleetRepository.GetAll();
}
