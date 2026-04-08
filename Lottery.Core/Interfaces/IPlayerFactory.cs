using Lottery.Core.Models;

namespace Lottery.Core.Interfaces
{
    public interface IPlayerFactory
    {
        IReadOnlyList<Player> BuildRoster(int totalPlayers);
    }
}
