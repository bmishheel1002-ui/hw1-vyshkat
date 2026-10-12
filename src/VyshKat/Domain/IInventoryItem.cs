namespace VyshKat.Domain;

/// <summary>
/// Describes an item recorded by the service.
/// </summary>
public interface IInventoryItem
{
    /// <summary>
    /// Gets the readable type of the item, such as Scooter or Helmet.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the unique number used to identify the item in the inventory.
    /// </summary>
    string InventoryNumber { get; }
}
