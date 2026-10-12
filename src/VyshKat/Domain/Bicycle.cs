namespace VyshKat.Domain;

/// <summary>
/// Represents a bicycle that can be rented.
/// </summary>
public sealed class Bicycle : RentalVehicle
{
    // Give the common constructor the bicycle name and the two values it needs.
    public Bicycle(string inventoryNumber, int beginnerFriendliness)
        : base("Bicycle", inventoryNumber, beginnerFriendliness)
    {
    }
}
