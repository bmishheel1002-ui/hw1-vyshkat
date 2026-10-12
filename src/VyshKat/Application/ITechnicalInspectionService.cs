using VyshKat.Domain;

namespace VyshKat.Application;

/// <summary>
/// Checks whether an item may join the fleet.
/// </summary>
public interface ITechnicalInspectionService
{
    /// <summary>
    /// Returns true when the item may join the fleet.
    /// </summary>
    bool Approves(IInventoryItem item);
}
