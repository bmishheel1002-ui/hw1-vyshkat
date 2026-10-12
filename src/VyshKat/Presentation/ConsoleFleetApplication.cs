using System.Globalization;
using VyshKat.Application;
using VyshKat.Domain;

namespace VyshKat.Presentation;

/// <summary>
/// Shows a fleet demo and accepts commands from the console.
/// </summary>
public sealed class ConsoleFleetApplication(
    FleetService fleetService,
    TextReader input,
    TextWriter output)
{
    /// <summary>
    /// Runs the sample demonstration, then starts the interactive menu.
    /// </summary>
    public void Run()
    {
        RunDemo();
        RunMenu();
    }

    // Add fixed examples so every main feature is visible at startup.
    private void RunDemo()
    {
        IInventoryItem[] items =
        [
            new Scooter("SC-001", beginnerFriendliness: 8),
            new Bicycle("BI-001", beginnerFriendliness: 5),
            new ElectricBicycle("EB-001", beginnerFriendliness: 9, dailyEnergyConsumptionKwh: 1.4m),
            new Helmet("HE-001"),
            new ChargingStation("CH-001", dailyEnergyConsumptionKwh: 4.2m),
            new ChargingStation("CH-002", dailyEnergyConsumptionKwh: 12m)
        ];

        output.WriteLine("VyshKat fleet demo");

        // Try to add each example and show the inspection result.
        foreach (var item in items)
        {
            var accepted = fleetService.TryAddToFleet(item);
            var status = accepted ? "accepted" : "rejected by technical inspection";
            output.WriteLine($"{item.Name} ({item.InventoryNumber}): {status}");
        }

        PrintReports();
    }

    // Keep showing the menu until the user exits or closes the input stream.
    private void RunMenu()
    {
        while (true)
        {
            output.WriteLine();
            output.WriteLine("Menu: 1 - add an item, 2 - show reports, 0 - exit");
            output.Write("Choose an option: ");
            var choice = input.ReadLine();

            if (choice is null || choice == "0")
            {
                return;
            }

            switch (choice)
            {
                case "1":
                    AddItemFromInput();
                    break;
                case "2":
                    PrintReports();
                    break;
                default:
                    output.WriteLine("Unknown menu option. Choose 1, 2, or 0.");
                    break;
            }
        }
    }

    // Ask for the item details, then let the fleet service inspect and save it.
    private void AddItemFromInput()
    {
        output.WriteLine("Item type: 1 - scooter, 2 - bicycle, 3 - electric bicycle, 4 - helmet, 5 - charging station");
        output.Write("Choose an item type: ");
        var type = input.ReadLine();

        if (type is null || type == "0")
        {
            output.WriteLine("Item entry cancelled.");
            return;
        }

        output.Write("Inventory number: ");
        var inventoryNumber = input.ReadLine();
        if (string.IsNullOrWhiteSpace(inventoryNumber))
        {
            output.WriteLine("Inventory number cannot be empty. Item was not added.");
            return;
        }

        var item = CreateItem(type, inventoryNumber);
        if (item is null)
        {
            return;
        }

        var accepted = fleetService.TryAddToFleet(item);
        output.WriteLine(accepted
            ? $"{item.Name} ({item.InventoryNumber}) was accepted and added."
            : "Item was not added: its number may already exist or the inspection rejected it.");
    }

    // Create the selected item after reading any values it needs.
    private IInventoryItem? CreateItem(string type, string inventoryNumber)
    {
        switch (type)
        {
            case "1":
            {
                var score = ReadBeginnerScore();
                return score is null ? null : new Scooter(inventoryNumber, score.Value);
            }
            case "2":
            {
                var score = ReadBeginnerScore();
                return score is null ? null : new Bicycle(inventoryNumber, score.Value);
            }
            case "3":
            {
                var score = ReadBeginnerScore();
                if (score is null)
                {
                    return null;
                }

                var energy = ReadEnergyUse();
                return energy is null ? null : new ElectricBicycle(inventoryNumber, score.Value, energy.Value);
            }
            case "4":
                return new Helmet(inventoryNumber);
            case "5":
            {
                var energy = ReadEnergyUse();
                return energy is null ? null : new ChargingStation(inventoryNumber, energy.Value);
            }
            default:
                output.WriteLine("Unknown item type. Item was not added.");
                return null;
        }
    }

    // Accept only a whole-number beginner score from 1 to 10.
    private int? ReadBeginnerScore()
    {
        output.Write("Beginner score (1-10): ");
        var text = input.ReadLine();
        if (int.TryParse(text, out var score) && score is >= 1 and <= 10)
        {
            return score;
        }

        output.WriteLine("Enter a whole number from 1 to 10. Item was not added.");
        return null;
    }

    // Accept non-negative energy values using either the current or invariant decimal format.
    private decimal? ReadEnergyUse()
    {
        output.Write("Daily energy use in kWh (for example, 1.5): ");
        var text = input.ReadLine();
        // Do not allow a dot to be read as a thousands separator in comma-decimal locales.
        const NumberStyles styles = NumberStyles.Float;
        var parsed = decimal.TryParse(text, styles, CultureInfo.CurrentCulture, out var energy)
            || decimal.TryParse(text, styles, CultureInfo.InvariantCulture, out energy);

        if (parsed && energy >= 0)
        {
            return energy;
        }

        output.WriteLine("Enter a non-negative number. Item was not added.");
        return null;
    }

    // Print all reports required by the homework using the current fleet data.
    private void PrintReports()
    {
        output.WriteLine();
        output.WriteLine($"Total accepted items: {fleetService.GetItemCount()}");
        output.WriteLine($"Daily energy consumption: {fleetService.GetDailyEnergyConsumptionKwh():0.##} kWh");

        output.WriteLine();
        output.WriteLine("Vehicles suitable for beginners:");
        foreach (var vehicle in fleetService.GetVehiclesForBeginners())
        {
            output.WriteLine($"- {vehicle.Name} ({vehicle.InventoryNumber}), score {vehicle.BeginnerFriendliness}/10");
        }

        output.WriteLine();
        output.WriteLine("All accepted inventory items:");
        foreach (var item in fleetService.GetInventoryItems())
        {
            output.WriteLine($"- {item.Name}: {item.InventoryNumber}");
        }
    }
}
