namespace Lottery.Core.Models
{
    public class Player
    {
        public int Id { get; init; } 
        public decimal Balance { get; set; }
        public List<LotteryTicket> LotteryTickets { get; set; } = new();
        public bool IsHuman { get; init; }
    }
}
