using VyshKat.Domain;

namespace VyshKat.Application;

/// <summary>
/// Provides storage operations for fleet items.
/// </summary>
public interface IFleetRepository
{
    /// <summary>
    /// Gets every item currently recorded in the fleet.
    /// </summary>
    IReadOnlyList<IInventoryItem> GetAll();

    /// <summary>
    /// Checks whether an item already uses this inventory number.
    /// </summary>
    bool ContainsInventoryNumber(string inventoryNumber);

    /// <summary>
    /// Saves an item in the fleet.
    /// </summary>
    void Add(IInventoryItem item);
}
