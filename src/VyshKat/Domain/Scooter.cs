namespace VyshKat.Domain;

/// <summary>
/// Represents a scooter that can be rented.
/// </summary>
public sealed class Scooter : RentalVehicle
{
    // Give the common constructor the scooter name and the two values it needs.
    public Scooter(string inventoryNumber, int beginnerFriendliness)
        : base("Scooter", inventoryNumber, beginnerFriendliness)
    {
    }
}
