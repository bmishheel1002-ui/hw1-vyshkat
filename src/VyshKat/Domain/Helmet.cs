namespace VyshKat.Domain;

/// <summary>
/// Represents a helmet tracked in inventory.
/// </summary>
public sealed class Helmet : IInventoryItem
{
    // A helmet only needs an inventory number when it is created.
    public Helmet(string inventoryNumber)
    {
        // Reject an empty inventory number.
        if (string.IsNullOrWhiteSpace(inventoryNumber))
        {
            throw new ArgumentException("Inventory number cannot be empty.", nameof(inventoryNumber));
        }

        // Keep the checked inventory number.
        InventoryNumber = inventoryNumber;
    }

    // Return the display name of this item.
    public string Name => "Helmet";

    // Return the identifier saved at creation.
    public string InventoryNumber { get; }
}
