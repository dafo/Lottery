using Lottery.Core.Models;

namespace Lottery.Core.Interfaces
{
    public interface ITicketService
    {
        void PurchaseTickets(Player player, int count);
    }
}
