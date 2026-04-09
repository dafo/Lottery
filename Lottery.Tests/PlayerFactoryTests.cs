using Lottery.Core.Factories;
using Lottery.Core.Models;

namespace Lottery.Tests
{
    public class PlayerFactoryTests
    {
        private readonly LotterySettings _settings = new()
        {
            StartingBalance = 10m,
            TicketPrice = 1m,
            MinNumberOfPlayers = 10,
            MaxNumberOfPlayers = 15,
            MinNumberOfTicketsPerPlayer = 1,
            MaxNumberOfTicketsPerPlayer = 10
        };

        [Fact]
        public void BuildRoster_FirstPlayerIsHuman()
        {
            var factory = new PlayerFactory(_settings);
            var players = factory.BuildRoster(_settings.MinNumberOfPlayers);

            var human = players[0];
            Assert.Equal(1, human.Id);
            Assert.True(human.IsHuman);
            Assert.Equal(10m, human.Balance);
        }

        [Fact]
        public void BuildRoster_OtherPlayersAreCpu()
        {
            var factory = new PlayerFactory(_settings);
            var players = factory.BuildRoster(_settings.MaxNumberOfPlayers);

            var cpuPlayers = players.Skip(1).ToList();
            Assert.True(cpuPlayers.All(p => !p.IsHuman));
        }

        [Fact]
        public void BuildRoster_ReturnsCorrectTotalCount()
        {
            var factory = new PlayerFactory(_settings);
            var players = factory.BuildRoster(12);
            Assert.Equal(12, players.Count);
        }

        [Fact]
        public void BuildRoster_CpuPlayersIdsAreSequential()
        {
            var factory = new PlayerFactory(_settings);
            var players = factory.BuildRoster(_settings.MinNumberOfPlayers);
            for (int i = 1; i < players.Count; i++)
            {
                Assert.Equal(i + 1, players[i].Id);
            }
        }

        [Fact]
        public void BuildRoster_CpuPlayersHaveStartingBalance()
        {
            var factory = new PlayerFactory(_settings);
            var players = factory.BuildRoster(_settings.MaxNumberOfPlayers);
            var cpuPlayers = players.Skip(1).ToList();
            Assert.True(cpuPlayers.All(p => p.Balance == _settings.StartingBalance));
        }
    }
}