# VyshKat - Homework 1

## What the program does

This C#/.NET 10 console program keeps a small fleet and its inventory. It demonstrates:

- adding an item only after a technical inspection;
- rejecting an item when its inventory number is already used;
- counting accepted items and adding their daily energy use;
- listing rental vehicles with a beginner score of 6 or more;
- showing each accepted item's name and inventory number;
- replacing the inspection service through dependency injection (DI).

## Run the program

From the `hw1-vyshkat` project directory, run:

```bash
dotnet restore src/VyshKat/VyshKat.csproj
dotnet run --project src/VyshKat/VyshKat.csproj
```

The demo tries to add six sample items. It accepts five and rejects the charging station that uses 12 kWh per day. The example inspection rule rejects an energy user above 10 kWh per day. This limit is a simple demonstration rule; it is not part of the business requirements.

After the demo, the app opens an interactive menu:

- `1` adds a scooter, bicycle, electric bicycle, helmet, or charging station;
- `2` prints the current fleet reports;
- `0` exits.

For rental vehicles, enter a beginner score from 1 to 10. For electric bicycles and charging stations, enter daily energy use in kWh. The app accepts decimal values with a dot or the current system's decimal separator. The 10 kWh inspection limit is only a demonstration rule, not a business requirement.

Expected summary:

```text
Total accepted items: 5
Daily energy consumption: 5.6 kWh
```

## Tests and code coverage

Run the fourteen unit tests:

```bash
dotnet test tests/VyshKat.Tests/VyshKat.Tests.csproj
```

Measure coverage with Coverlet:

```bash
dotnet test tests/VyshKat.Tests/VyshKat.Tests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
```

Verification: all 14 tests pass. Measured coverage is **83.19% line, 77.27% branch, and 94.59% method**, above the 60% line-coverage benchmark. To reproduce from the 'hw1-vyshkat' directory:

```bash
#Test + coverege report
dotnet test tests/VyshKat.Tests/VyshKat.Tests.csproj --no-restore /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=TestResults/coverage.cobertura.xml

#Interactive session: add a scooter, print reports, exit
printf '1\n1\nNEW-001\n8\n2\n0\n' | dotnet run --no-build --project src/VyshKat/VyshKat.csproj
```

The coverage report is created at `tests/VyshKat.Tests/TestResults/coverage.cobertura.xml`.

The tests check accepted and rejected items, duplicate inventory numbers, energy totals, the beginner rule, DI replacement, invalid console input and interactive item addition.

## How the code works

1. **Domain** (`src/VyshKat/Domain/`) describes the items. `IInventoryItem` means an item has a name and inventory number. `IEnergyConsumer` means it uses energy. `IRentalVehicle` means it can be rented and has a beginner score. Scooters and bicycles share checks in `RentalVehicle`. A helmet is only an inventory item; an electric bicycle and charging station also use energy.
2. **Inspection** (`TechnicalInspectionService.cs`) decides whether an item can enter the fleet. It rejects energy use above 10 kWh per day.
3. **Storage** (`InMemoryFleetRepository.cs`) keeps accepted items in a list while the program runs.
4. **Fleet actions** (`FleetService.cs`) checks for duplicate inventory numbers, asks for inspection, stores accepted items, and prepares the reports.
5. **Console interface** (`src/VyshKat/Presentation/ConsoleFleetApplication.cs`) runs the sample demo, reads menu choices and item details, and prints reports. It receives `FleetService`, input, and output through its constructor, so tests can provide fake input and capture output.
6. **Program setup** (`Program.cs`) registers the services in the DI container and starts the console interface.
7. **Tests** (`tests/VyshKat.Tests/FleetServiceTests.cs`) use `FakeInspectionService` to choose accept or reject answers without changing the production inspection rules. They also run the console interface with test input.

## Answers to the mandatory questions

1. **Which SOLID principles are visible?** DIP is clearest in `FleetService`: it receives `IFleetRepository` and `ITechnicalInspectionService` instead of creating concrete dependencies. The console interface also receives its input and output streams, which makes it testable. ISP is shown by the small `IInventoryItem`, `IEnergyConsumer`, and `IRentalVehicle` interfaces, so an item only needs the capabilities that apply to it. OCP is supported in the fleet logic: new item classes can implement these abstractions without adding type-specific rules to `FleetService`; the console's item menu must also be extended to let users create a new type.
2. **Which principle is less visible, and why?** SRP is the least clear. `FleetService` handles adding items and preparing several fleet reports. They use the same fleet data, so they are kept together in this small homework instead of creating extra classes. A larger program might separate them if they change for different reasons.
3. **What happens when we add a vehicle, employee, or parts warehouse?** A new vehicle can inherit from `RentalVehicle`; fleet logic can work with it through `IRentalVehicle` and `IInventoryItem`, without a new vehicle-specific rule in `FleetService`. To let users add it, the console item menu must get a new choice. Employees are not inventory items under the domain rules, so they should have their own model and service. A parts warehouse would be a separate domain area with its own model and storage, rather than a new case inside `FleetService`.
4. **Which tests check inspection or the beginner rule, and how is inspection replaced?** `TryAddToFleet_does_not_add_item_when_inspection_rejects_it` verifies that a rejected item is not saved. `Technical_inspection_rejects_item_above_energy_limit` checks the real inspection rule. `Beginner_vehicle_report_uses_threshold_six` checks the beginner score rule. The fake implements `ITechnicalInspectionService` and is passed to `FleetService`; the DI test registers that fake in place of the real service.
5. **What did AI do, and what did you change?** I used AI help to review the project structure, tests, and English comments. I asked for explanations to help me understand the code. I ran the project tests; the current result is shown in the coverage section above.

**Changes made:** With AI help, I organized the project into Domain, Application, and Infrastructure parts and added the fleet rules, tests, comments, and documentation. I reviewed the code and used the explanations to understand the design.

## Project map

```text
src/VyshKat/Domain/          item types and interfaces
src/VyshKat/Application/     fleet actions and inspection
src/VyshKat/Infrastructure/  in-memory storage
src/VyshKat/Presentation/    demo, interactive menu, and console reports
src/VyshKat/Program.cs       DI setup and application startup
tests/VyshKat.Tests/         unit tests
```
