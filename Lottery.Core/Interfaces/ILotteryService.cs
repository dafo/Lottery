using Lottery.Core.Models;

namespace Lottery.Core.Interfaces
{
    public interface ILotteryService
    {
        void SetupPlayers();
        void PurchaseTickets(int humanTicketCount);
        IReadOnlyList<Player> GetPlayers();
        DrawResult RunDraw();
    }
}
