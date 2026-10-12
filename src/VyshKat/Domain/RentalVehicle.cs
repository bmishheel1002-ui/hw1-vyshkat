namespace VyshKat.Domain;

/// <summary>
/// Stores the data shared by all rental vehicles.
/// </summary>
public abstract class RentalVehicle : IRentalVehicle
{
    // The protected constructor lets child vehicle classes provide their data.
    protected RentalVehicle(string name, string inventoryNumber, int beginnerFriendliness)
    {
        // A vehicle must have a name.
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        }

        // A vehicle must have an inventory number.
        if (string.IsNullOrWhiteSpace(inventoryNumber))
        {
            throw new ArgumentException("Inventory number cannot be empty.", nameof(inventoryNumber));
        }

        // The score is only valid from 1 to 10.
        if (beginnerFriendliness is < 1 or > 10)
        {
            throw new ArgumentOutOfRangeException(
                nameof(beginnerFriendliness),
                beginnerFriendliness,
                "Beginner friendliness must be between 1 and 10.");
        }
        
        // Save the checked values.
        Name = name;
        InventoryNumber = inventoryNumber;
        BeginnerFriendliness = beginnerFriendliness;
    }

    // The vehicle name does not change after creation.
    public string Name { get; }

    // The inventory number does not change after creation.
    public string InventoryNumber { get; }

    // The beginner score does not change after creation.
    public int BeginnerFriendliness { get; }
}
