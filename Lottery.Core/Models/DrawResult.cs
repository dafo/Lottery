namespace Lottery.Core.Models
{
    public class DrawResult
    {
        public decimal TotalRevenue { get; init; }
        public decimal HouseProfit { get; init; }
        public IReadOnlyList<TierResult> TierResults { get; init; } = [];
    }

    public class TierResult
    {
        public string TierName { get; init; } = string.Empty;
        public IReadOnlyList<Winner> Winners { get; init; } = [];
    }

    public class Winner { 
        public int PlayerId { get; init; }
        public int NumberOfWinningTickets { get; init; }
        public decimal PrizePerTicket { get; init; }
        public decimal TotalPrize { get; init; }

        public string Display() => $"{PlayerId}({NumberOfWinningTickets})";
    }
}
