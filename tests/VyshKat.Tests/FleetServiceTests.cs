using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using VyshKat.Application;
using VyshKat.Domain;
using VyshKat.Infrastructure;
using VyshKat.Presentation;
using Xunit;

namespace VyshKat.Tests;

// These tests check the fleet rules without starting the console program.
public sealed class FleetServiceTests
{
    // An approved item should be saved and counted.
    [Fact]
    public void TryAddToFleet_adds_item_when_inspection_approves_it()
    {
        // Arrangement of making an inspection that always approves and a sample scooter.
        var inspection = new FakeInspectionService(_ => true);
        var service = CreateService(inspection);
        var scooter = new Scooter("SC-001", beginnerFriendliness: 8);

        // Act: try to add the scooter.
        var accepted = service.TryAddToFleet(scooter);

        // Assert: check success, storage, count, and inspection call.
        Assert.True(accepted);
        Assert.Single(service.GetInventoryItems());
        Assert.Equal(1, service.GetItemCount());
        Assert.Equal(1, inspection.Calls);
    }

    // A rejected item should not be saved.
    [Fact]
    public void TryAddToFleet_does_not_add_item_when_inspection_rejects_it()
    {
        // Arrangement of using an inspection that always rejects.
        var service = CreateService(new FakeInspectionService(_ => false));
        var scooter = new Scooter("SC-001", beginnerFriendliness: 8);

        // Act: ask the service to add the scooter.
        var accepted = service.TryAddToFleet(scooter);

        // Assert: the service reports failure and the repository stays empty.
        Assert.False(accepted);
        Assert.Empty(service.GetInventoryItems());
    }

    // Two items cannot use the same inventory number.
    [Fact]
    public void TryAddToFleet_rejects_duplicate_inventory_numbers()
    {
        // Arrangement of preparing two scooters with the same number.
        var service = CreateService(new FakeInspectionService(_ => true));
        var firstScooter = new Scooter("SC-001", beginnerFriendliness: 8);
        var secondScooter = new Scooter("SC-001", beginnerFriendliness: 9);

        // Act and assert: the first item is saved; the second is rejected.
        Assert.True(service.TryAddToFleet(firstScooter));
        Assert.False(service.TryAddToFleet(secondScooter));
        Assert.Single(service.GetInventoryItems());
    }

    // The energy report should add energy users and ignore other items.
    [Fact]
    public void Daily_energy_consumption_sums_all_accepted_energy_consumers()
    {
        // Arrangement of adding an electric bicycle, a charging station, and a helmet.
        var service = CreateService(new FakeInspectionService(_ => true));
        service.TryAddToFleet(new ElectricBicycle("EB-001", 8, 1.5m));
        service.TryAddToFleet(new ChargingStation("CH-001", 4.25m));
        service.TryAddToFleet(new Helmet("HE-001"));

        // Act: calculate the total daily energy use.
        var total = service.GetDailyEnergyConsumptionKwh();

        // Assert: 1.5 + 4.25 equals 5.75; the helmet adds no energy.
        Assert.Equal(5.75m, total);
    }

    // A score of six passes, while a score of five does not.
    [Fact]
    public void Beginner_vehicle_report_uses_threshold_six()
    {
        // Arrangement of adding two vehicles on different sides of the score limit.
        var service = CreateService(new FakeInspectionService(_ => true));
        service.TryAddToFleet(new Scooter("SC-001", 6));
        service.TryAddToFleet(new Bicycle("BI-001", 5));

        // Act: request the beginner-friendly vehicles.
        var vehicles = service.GetVehiclesForBeginners();

        // Assert: only the scooter with score six is in the result.
        var vehicle = Assert.Single(vehicles);
        Assert.Equal("SC-001", vehicle.InventoryNumber);
    }

    // The DI container should be able to provide a fake inspection service.
    [Fact]
    public void Dependency_injection_container_can_replace_inspection_service()
    {
        // Arrangement of registering the fake instead of the production inspection service.
        var services = new ServiceCollection();
        services.AddSingleton<IFleetRepository, InMemoryFleetRepository>();
        services.AddSingleton<ITechnicalInspectionService>(new FakeInspectionService(_ => false));
        services.AddSingleton<FleetService>();

        // Build the container and ask it to create the fleet service.
        using var provider = services.BuildServiceProvider();
        var service = provider.GetRequiredService<FleetService>();

        // Act: try to add an item; the fake inspection will reject it.
        var accepted = service.TryAddToFleet(new Scooter("SC-001", 8));

        // Assert: the rejected item was not saved.
        Assert.False(accepted);
        Assert.Empty(service.GetInventoryItems());
    }

    // The real inspection service should reject energy use above its limit.
    [Fact]
    public void Technical_inspection_rejects_item_above_energy_limit()
    {
        // Arrangement of creating the production inspection service.
        var inspection = new TechnicalInspectionService();

        // Act: check a station that uses more than 10 kWh each day.
        var accepted = inspection.Approves(new ChargingStation("CH-001", 10.01m));

        // Assert: the station must not be accepted.
        Assert.False(accepted);
    }

    // The inspection should accept an energy user exactly at the configured limit.
    [Fact]
    public void Technical_inspection_accepts_item_at_exactly_energy_limit()
    {
        var inspection = new TechnicalInspectionService();

        var accepted = inspection.Approves(new ChargingStation("CH-002", 10m));

        Assert.True(accepted);
    }

    // A new fleet should report zero daily energy use.
    [Fact]
    public void Daily_energy_consumption_is_zero_on_empty_fleet()
    {
        var service = CreateService(new FakeInspectionService(_ => true));

        Assert.Equal(0m, service.GetDailyEnergyConsumptionKwh());
    }

    // Rental vehicles must reject beginner scores outside the allowed range.
    [Fact]
    public void Rental_vehicle_rejects_beginner_friendliness_out_of_range()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new Scooter("SC-002", beginnerFriendliness: 0));
    }

    // The app should accept an item typed by the user and show the new reports.
    [Fact]
    public void Console_application_adds_user_item_and_shows_updated_report()
    {
        var output = new StringWriter();
        var application = CreateConsoleApplication("1\n1\nUSER-001\n8\n2\n0\n", output);

        application.Run();

        Assert.Contains("Scooter (USER-001) was accepted and added.", output.ToString());
        Assert.Contains("Total accepted items: 6", output.ToString());
        Assert.Contains("Scooter (USER-001)", output.ToString());
    }

    // The app should reject bad values and continue to accept menu commands.
    [Fact]
    public void Console_application_handles_invalid_input_and_continues()
    {
        var output = new StringWriter();
        var application = CreateConsoleApplication(
            "9\n1\n9\nINVALID-TYPE\n1\n1\nUSER-002\n12\n1\n5\nST-001\n-1\n2\n0\n",
            output);

        application.Run();

        Assert.Contains("Unknown menu option", output.ToString());
        Assert.Contains("Unknown item type", output.ToString());
        Assert.Contains("Enter a whole number from 1 to 10", output.ToString());
        Assert.Contains("Enter a non-negative number", output.ToString());
        Assert.Contains("Total accepted items: 5", output.ToString());
    }

    // The app should report duplicate numbers and items rejected by inspection.
    [Fact]
    public void Console_application_reports_duplicate_and_inspection_rejections()
    {
        var output = new StringWriter();
        var application = CreateConsoleApplication(
            "1\n1\nSC-001\n8\n1\n5\nST-002\n12\n0\n",
            output);

        application.Run();

        Assert.Contains("number may already exist or the inspection rejected it", output.ToString());
        Assert.Contains("Total accepted items: 5", output.ToString());
    }

    // A dot decimal should still mean one and a half in a comma-decimal locale.
    [Fact]
    public void Console_application_reads_dot_decimal_in_russian_locale()
    {
        var previousCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
            var output = new StringWriter();
            var application = CreateConsoleApplication("1\n3\nUSER-003\n8\n1.5\n2\n0\n", output);

            application.Run();

            Assert.Contains("Daily energy consumption: 7,1 kWh", output.ToString());
        }
        finally
        {
            CultureInfo.CurrentCulture = previousCulture;
        }
    }

    // Create a console app with test input and captured output instead of the real terminal.
    private static ConsoleFleetApplication CreateConsoleApplication(string text, TextWriter output)
    {
        var repository = new InMemoryFleetRepository();
        var service = new FleetService(repository, new TechnicalInspectionService());
        return new ConsoleFleetApplication(service, new StringReader(text), output);
    }

    // Make a service with a fresh in-memory store and the chosen inspection.
    private static FleetService CreateService(ITechnicalInspectionService inspectionService)
    {
        return new FleetService(new InMemoryFleetRepository(), inspectionService);
    }

    // This test helper follows the inspection interface but uses a chosen answer.
    private sealed class FakeInspectionService(Func<IInventoryItem, bool> decision)
        : ITechnicalInspectionService
    {
        // Count how many times the fleet asks for an inspection decision.
        public int Calls { get; private set; }

        // Record the call and return the answer chosen by the test.
        public bool Approves(IInventoryItem item)
        {
            Calls++;
            return decision(item);
        }
    }
}
