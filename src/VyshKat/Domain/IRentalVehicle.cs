namespace VyshKat.Domain;

/// <summary>
/// Describes a rental vehicle that is also an inventory item.
/// </summary>
public interface IRentalVehicle : IInventoryItem
{
    /// <summary>
    /// Gets a score from 1 to 10 for new riders.
    /// </summary>
    int BeginnerFriendliness { get; }
}
