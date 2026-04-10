using Lottery.Core.Models;

namespace Lottery
{
    public static class ConsoleDisplay
    {
        public static void ShowWelcome(LotterySettings settings)
        {
            Console.WriteLine("Welcome to the Bede Lottery, Player 1!");
            Console.WriteLine($"* Your digital balance: {settings.StartingBalance:C}");
            Console.WriteLine($"* Ticket Price: {settings.TicketPrice:C} each\n");
        }

        public static int AskTicketCount(LotterySettings settings)
        {
            Console.Write($"How many tickets do you want to buy, Player 1? ({settings.MinNumberOfTicketsPerPlayer}-{settings.MaxNumberOfTicketsPerPlayer}): ");

            if (!int.TryParse(Console.ReadLine(), out var count) || count < settings.MinNumberOfTicketsPerPlayer)
            {
                Console.WriteLine($"Invalid input. Defaulting to {settings.MinNumberOfTicketsPerPlayer} ticket.");
                return settings.MinNumberOfTicketsPerPlayer;
            }

            if (count > settings.MaxNumberOfTicketsPerPlayer)
            {
                Console.WriteLine($"Requested {count} exceeds the maximum. Buying {settings.MaxNumberOfTicketsPerPlayer} tickets instead.");
                return settings.MaxNumberOfTicketsPerPlayer;
            }

            return count;
        }

        public static void ShowResults(DrawResult result, int cpuCount)
        {
            Console.WriteLine($"\n{cpuCount} other CPU players also have purchased tickets.\n");
            Console.WriteLine("Ticket Draw Results:\n");

            foreach (var tier in result.TierResults)
            {
                if (tier.Winners.Count == 0)
                {
                    Console.WriteLine($"* {tier.TierName}: No winners");
                    continue;
                }

                var totalWinningTickets = tier.Winners.Sum(w => w.NumberOfWinningTickets);

                if (totalWinningTickets == 1)
                {
                    var winner = tier.Winners[0];
                    Console.WriteLine($"* {tier.TierName}: Player {winner.Display()} wins {winner.TotalPrize:C}!");
                }
                else
                {
                    var prizePerTicket = tier.Winners[0].PrizePerTicket;
                    var playerList = string.Join(", ", tier.Winners.OrderBy(w => w.PlayerId).Select(w => w.Display()));
                    Console.WriteLine($"* {tier.TierName}: Players {playerList} win {prizePerTicket:C} per winning ticket!");
                }
            }

            Console.WriteLine($"\nCongratulations to the winners!\nHouse profit: {result.HouseProfit:C}");
        }
    }
}
