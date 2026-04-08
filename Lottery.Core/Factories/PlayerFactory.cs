using Lottery.Core.Interfaces;
using Lottery.Core.Models;

namespace Lottery.Core.Factories
{
    public class PlayerFactory : IPlayerFactory
    {
        private readonly LotterySettings _settings;
        public PlayerFactory(LotterySettings settings)
        {
            _settings = settings;
        }

        public IReadOnlyList<Player> BuildRoster(int totalPlayers)
        {
            var players = new List<Player>
            {
                new Player
                {
                    Id = 1,
                    Balance = _settings.StartingBalance,
                    IsHuman = true
                }
            };
            for (int i = 2; i <= totalPlayers; i++)
            {
                players.Add(new Player
                {
                    Id = i,
                    Balance = _settings.StartingBalance,
                    IsHuman = false
                });
            }

            return players.AsReadOnly();
        }
    }
}
