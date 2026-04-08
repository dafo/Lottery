namespace Lottery.Core.Models
{
    public class LotteryTicket
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public int OwnerId { get; init; }
    }
}
