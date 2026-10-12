using Microsoft.Extensions.DependencyInjection;
using VyshKat.Application;
using VyshKat.Infrastructure;
using VyshKat.Presentation;

// Create the list of services that the dependency-injection container will manage.
var services = new ServiceCollection();
// Use the in-memory store whenever the app asks for the repository interface.
services.AddSingleton<IFleetRepository, InMemoryFleetRepository>();
// Use this inspection policy whenever the app asks for the inspection interface.
services.AddSingleton<ITechnicalInspectionService, TechnicalInspectionService>();
// Ask the container to create FleetService and provide its two dependencies.
services.AddSingleton<FleetService>();
// Give the console application the input and output streams used by this process.
services.AddSingleton<TextReader>(_ => Console.In);
services.AddSingleton<TextWriter>(_ => Console.Out);
services.AddSingleton<ConsoleFleetApplication>();

// Build the container and dispose it automatically when the program ends.
using var serviceProvider = services.BuildServiceProvider();
// Run the sample first, then let the user work with the fleet from the console.
serviceProvider.GetRequiredService<ConsoleFleetApplication>().Run();
