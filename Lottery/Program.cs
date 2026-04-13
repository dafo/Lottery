using Lottery;
using Lottery.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

try
{
    var settings = GameSetup.LoadSettings();
    using var services = GameSetup.BuildServiceProvider(settings);

    var lottery = services.GetRequiredService<ILotteryService>();

    ConsoleDisplay.ShowWelcome(settings);
    lottery.SetupPlayers();

    var ticketCount = ConsoleDisplay.AskTicketCount(settings);
    lottery.PurchaseTickets(ticketCount);

    var result = lottery.RunDraw();
    var cpuCount = lottery.GetPlayers().Count(p => !p.IsHuman);

    ConsoleDisplay.ShowResults(result, cpuCount);
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Configuration error: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"An unexpected error occurred: {ex.Message}");
}
