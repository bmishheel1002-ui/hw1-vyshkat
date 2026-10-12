using VyshKat.Application;
using VyshKat.Domain;

namespace VyshKat.Infrastructure;

/// <summary>
/// Keeps fleet items in memory while the program is running.
/// </summary>
public sealed class InMemoryFleetRepository : IFleetRepository
{
    // The private list is the storage for this demo.
    private readonly List<IInventoryItem> _items = [];

    // Give callers a read-only view of the saved items.
    public IReadOnlyList<IInventoryItem> GetAll() => _items.AsReadOnly();

    // Return true when any saved item has the same number.
    public bool ContainsInventoryNumber(string inventoryNumber) =>
        _items.Any(item => item.InventoryNumber == inventoryNumber);

    // Add the item to the private list.
    public void Add(IInventoryItem item) => _items.Add(item);
}
