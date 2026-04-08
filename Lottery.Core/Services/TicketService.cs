using Lottery.Core.Interfaces;
using Lottery.Core.Models;

namespace Lottery.Core.Services
{
    public class TicketService : ITicketService
    {
        private readonly LotterySettings _settings;

        public TicketService(LotterySettings settings)
        {
            _settings = settings;
        }

        public void PurchaseTickets(Player player, int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (player.Balance < _settings.TicketPrice)
                    break;

                player.Balance -= _settings.TicketPrice;
                    player.LotteryTickets.Add(new LotteryTicket { OwnerId = player.Id });
            }   
        }
    }
}
