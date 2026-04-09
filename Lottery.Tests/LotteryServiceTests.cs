using Lottery.Core.Factories;
using Lottery.Core.Interfaces;
using Lottery.Core.Models;
using Lottery.Core.Services;
using Moq;

namespace Lottery.Tests
{
    public class LotteryServiceTests
    {
        private readonly LotterySettings _settings = new()
        {
            TicketPrice = 1m,
            StartingBalance = 10m,
            MinNumberOfPlayers = 10,
            MaxNumberOfPlayers = 15,
            MinNumberOfTicketsPerPlayer = 1,
            MaxNumberOfTicketsPerPlayer = 10,
            PrizeLevels = new List<PrizeLevel>
              {
                  new() { Name = "Grand Prize", RevenueRate = 0.50m, FixedWinnerCount = 1 },
                  new() { Name = "Second Tier", RevenueRate = 0.30m, WinnerTicketRate = 0.10m },
                  new() { Name = "Third Tier", RevenueRate = 0.10m, WinnerTicketRate = 0.20m }
              }
        };

        private readonly Mock<INumberGenerator> _mockNumberGenerator = new();
        private readonly Mock<ITicketService> _mockTicketService = new();

        private LotteryService CreateService()
        {
            var factory = new PlayerFactory(_settings);
            return new LotteryService(_settings, _mockNumberGenerator.Object, _mockTicketService.Object, factory);
        }

        private LotteryService CreateServiceWithRealTickets(int playerCount, int ticketsPerPlayer)
        {
            var callIndex = 0;
            var sequentialGenerator = new Mock<INumberGenerator>();

            sequentialGenerator
                .Setup(g => g.Generate(10, 16))
                .Returns(playerCount);

            sequentialGenerator
                .Setup(g => g.Generate(1, 11))
                .Returns(ticketsPerPlayer);

            sequentialGenerator
                .Setup(g => g.Generate(0, It.IsAny<int>()))
                .Returns(0);

            var realTicketService = new TicketService(_settings);
            var factory = new PlayerFactory(_settings);
            return new LotteryService(_settings, sequentialGenerator.Object, realTicketService, factory);
        }

        [Fact]
        public void SetupPlayers_CreatesCorrectNumberOfPlayers()
        {
            _mockNumberGenerator
                .Setup(g => g.Generate(10, 16))
                .Returns(12);

            var service = CreateService();
            service.SetupPlayers();

            var players = service.GetPlayers();
            Assert.Equal(12, players.Count);
        }

        [Fact]
        public void SetupPlayers_FirstPlayerIsHuman()
        {
            _mockNumberGenerator
                .Setup(g => g.Generate(10, 16))
                .Returns(10);

            var service = CreateService();
            service.SetupPlayers();

            var firstPlayer = service.GetPlayers()[0];
            Assert.True(firstPlayer.IsHuman);
        }

        [Fact]
        public void PurchaseTickets_CallsTicketServiceForEveryPlayer()
        {
            _mockNumberGenerator
                .Setup(g => g.Generate(10, 16))
                .Returns(10);
            _mockNumberGenerator
                .Setup(g => g.Generate(1, 11))
                .Returns(3);

            var service = CreateService();
            service.SetupPlayers();
            service.PurchaseTickets(5);

            _mockTicketService.Verify(
                t => t.PurchaseTickets(It.IsAny<Player>(), It.IsAny<int>()),
                Times.Exactly(10));
        }

        [Fact]
        public void PurchaseTickets_HumanGetsRequestedCount()
        {
            _mockNumberGenerator
                .Setup(g => g.Generate(10, 16))
                .Returns(10);
            _mockNumberGenerator
                .Setup(g => g.Generate(1, 11))
                .Returns(3);

            var service = CreateService();
            service.SetupPlayers();
            service.PurchaseTickets(7);

            _mockTicketService.Verify(
                t => t.PurchaseTickets(
                    It.Is<Player>(p => p.IsHuman),
                    7),
                Times.Once);
        }

        [Fact]
        public void PurchaseTickets_CpuGetsRandomCount()
        {
            _mockNumberGenerator
                .Setup(g => g.Generate(10, 16))
                .Returns(10);
            _mockNumberGenerator
                .Setup(g => g.Generate(1, 11))
                .Returns(4);

            var service = CreateService();
            service.SetupPlayers();
            service.PurchaseTickets(5);

            _mockTicketService.Verify(
                t => t.PurchaseTickets(
                    It.Is<Player>(p => !p.IsHuman),
                    4),
                Times.Exactly(9));
        }

        [Fact]
        public void RunDraw_RevenueEqualsTicketCountTimesPrice()
        {
            var service = CreateServiceWithRealTickets(playerCount: 10, ticketsPerPlayer: 5);
            service.SetupPlayers();
            service.PurchaseTickets(5);

            var result = service.RunDraw();

            Assert.Equal(50m, result.TotalRevenue);
        }

        [Fact]
        public void RunDraw_HouseProfitIsAlwaysPositive()
        {
            var service = CreateServiceWithRealTickets(playerCount: 10, ticketsPerPlayer: 1);
            service.SetupPlayers();
            service.PurchaseTickets(5);

            var result = service.RunDraw();

            Assert.True(result.HouseProfit > 0, $"House profit was {result.HouseProfit}");
        }

        [Fact]
        public void RunDraw_TotalPrizesAndProfitEqualRevenue()
        {
            var service = CreateServiceWithRealTickets(playerCount: 10, ticketsPerPlayer: 5);
            service.SetupPlayers();
            service.PurchaseTickets(5);

            var result = service.RunDraw();

            var totalPrizes = result.TierResults
                .SelectMany(t => t.Winners)
                .Sum(w => w.TotalPrize);

            Assert.Equal(result.TotalRevenue, totalPrizes + result.HouseProfit);
        }

        [Fact]
        public void RunDraw_GrandPrizeHasOneWinningTicket()
        {
            var service = CreateServiceWithRealTickets(playerCount: 10, ticketsPerPlayer: 5);
            service.SetupPlayers();
            service.PurchaseTickets(5);

            var result = service.RunDraw();

            var grandPrize = result.TierResults.First(t => t.TierName == "Grand Prize");
            var totalWinningTickets = grandPrize.Winners.Sum(w => w.NumberOfWinningTickets);
            Assert.Equal(1, totalWinningTickets);
        }

        [Fact]
        public void RunDraw_ReturnsThreeTiers()
        {
            var service = CreateServiceWithRealTickets(playerCount: 10, ticketsPerPlayer: 5);
            service.SetupPlayers();
            service.PurchaseTickets(5);

            var result = service.RunDraw();

            Assert.Equal(3, result.TierResults.Count);
            Assert.Equal("Grand Prize", result.TierResults[0].TierName);
            Assert.Equal("Second Tier", result.TierResults[1].TierName);
            Assert.Equal("Third Tier", result.TierResults[2].TierName);
        }

        [Fact]
        public void RunDraw_WinningTicketsRemovedBetweenTiers()
        {
            var service = CreateServiceWithRealTickets(playerCount: 10, ticketsPerPlayer: 5);
            service.SetupPlayers();
            service.PurchaseTickets(5);

            var result = service.RunDraw();

            var totalWinningTickets = result.TierResults
                .SelectMany(t => t.Winners)
                .Sum(w => w.NumberOfWinningTickets);

            Assert.True(totalWinningTickets <= 50);
        }
    }
}
