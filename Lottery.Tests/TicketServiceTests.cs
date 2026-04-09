using Lottery.Core.Models;
using Lottery.Core.Services;

namespace Lottery.Tests
{
    public class TicketServiceTests
    {
        private readonly LotterySettings _settings = new()
        {
            TicketPrice = 1m,
            StartingBalance = 10m
        };

        private Player CreatePlayer(decimal balance = 10m)
        {
            return new Player
            {
                Id = 1,
                IsHuman = true,
                Balance = balance
            };
        }

        [Fact]
        public void PurchaseTickets_AddsCorrectNumberOfTickets()
        {
            var service = new TicketService(_settings);
            var player = CreatePlayer();

            service.PurchaseTickets(player, 7);
            Assert.Equal(7, player.LotteryTickets.Count);
        }

        [Fact]
        public void PurchaseTickets_DeductsBalanceCorrectly()
        {
            var service = new TicketService(_settings);
            var player = CreatePlayer();

            service.PurchaseTickets(player, 3);
            Assert.Equal(7m, player.Balance);
        }

        [Fact]
        public void PurchaseTickets_NotAllowedWhenBalanceInsufficient()
        {
            var service = new TicketService(_settings);
            var player = CreatePlayer(balance: 10m);

            service.PurchaseTickets(player, 11);
            Assert.Equal(10m, player.LotteryTickets.Count);
            Assert.Equal(0m, player.Balance);
        }

        [Fact]
        public void PurchaseTickets_TicketsHaveCorrectOwnerId()
        {
            var service = new TicketService(_settings);
            var player = CreatePlayer();

            service.PurchaseTickets(player, 3);
            Assert.All(player.LotteryTickets, t => Assert.Equal(t.OwnerId, player.Id));
        }

        [Fact]
        public void PurchaseTickets_TicketsHaveUniqueIds()
        {
            var service = new TicketService(_settings);
            var player = CreatePlayer();

            service.PurchaseTickets(player, 10);

            var ids = player.LotteryTickets.Select(t => t.Id).ToList();
            Assert.Equal(ids.Count, ids.Distinct().Count());
        }

        [Fact]
        public void PurchaseTickets_ZeroCount_NoTicketsAdded()
        {
            var service = new TicketService(_settings);
            var player = CreatePlayer();

            service.PurchaseTickets(player, 0);

            Assert.Empty(player.LotteryTickets);
            Assert.Equal(10m, player.Balance);
        }
    }
}
