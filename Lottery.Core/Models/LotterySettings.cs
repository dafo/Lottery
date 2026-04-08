namespace Lottery.Core.Models
{
    public class LotterySettings
    {
        public decimal TicketPrice { get; set; }
        public decimal StartingBalance { get; set; }
        public int MinNumberOfPlayers { get; set; }
        public int MaxNumberOfPlayers { get; set; }
        public int MinNumberOfTicketsPerPlayer { get; set; }
        public int MaxNumberOfTicketsPerPlayer { get; set; }
        public List<PrizeLevel> PrizeLevels { get; set; } = new();
    }

    public class PrizeLevel
    {
        public string Name { get; set; } = string.Empty;
        public decimal RevenueRate { get; set; }
        public int? FixedWinnerCount { get; set; }
        public decimal? WinnerTicketRate { get; set; }
    }
}
