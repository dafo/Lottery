using Lottery.Core.Interfaces;
using Lottery.Core.Models;

namespace Lottery.Core.Services
{
    public class LotteryService : ILotteryService
    {
        private readonly LotterySettings _settings;
        private readonly INumberGenerator _numberGenerator;
        private readonly ITicketService _ticketService;
        private readonly IPlayerFactory _playerFactory;
        private List<Player> _players = new();

        public LotteryService(
              LotterySettings settings,
              INumberGenerator numberGenerator,
              ITicketService ticketService,
              IPlayerFactory playerFactory)
        {
            _settings = settings;
            _numberGenerator = numberGenerator;
            _ticketService = ticketService;
            _playerFactory = playerFactory;
        }

        public void SetupPlayers()
        {
            var totalPlayers = _numberGenerator.Generate(
                _settings.MinNumberOfPlayers,
                _settings.MaxNumberOfPlayers + 1);

            _players = _playerFactory.BuildRoster(totalPlayers).ToList();
        }

        public IReadOnlyList<Player> GetPlayers() => _players.AsReadOnly();

        public void PurchaseTickets(int humanTicketCount)
        {
            foreach (var player in _players)
            {
                var count = player.IsHuman
                    ? humanTicketCount
                    : _numberGenerator.Generate(
                        _settings.MinNumberOfTicketsPerPlayer,
                        _settings.MaxNumberOfTicketsPerPlayer + 1);

                _ticketService.PurchaseTickets(player, count);
            }
        }

        public DrawResult RunDraw()
        {
            var availableTickets = _players.SelectMany(p => p.LotteryTickets).ToList();
            var totalTicketCount = availableTickets.Count;
            var totalRevenue = totalTicketCount * _settings.TicketPrice;
            var tierResults = new List<TierResult>();
            var totalPrizesAwarded = 0m;

            foreach (var prizeLevel in _settings.PrizeLevels)
            {
                var prizePool = totalRevenue * prizeLevel.RevenueRate;
                var winnerCount = DetermineWinnerCount(prizeLevel, totalTicketCount);

                if (winnerCount == 0 || availableTickets.Count == 0)
                {
                    tierResults.Add(new TierResult { TierName = prizeLevel.Name });
                    continue;
                }

                winnerCount = Math.Min(winnerCount, availableTickets.Count);

                var drawnTickets = DrawTickets(availableTickets, winnerCount);

                var prizePerTicket = Math.Truncate(prizePool / winnerCount * 100) / 100;
                totalPrizesAwarded += prizePerTicket * winnerCount;

                var winners = drawnTickets
                    .GroupBy(t => t.OwnerId)
                    .Select(g =>
                    {
                        var ticketsWon = g.Count();
                        return new Winner
                        {
                            PlayerId = g.Key,
                            NumberOfWinningTickets = ticketsWon,
                            PrizePerTicket = prizePerTicket,
                            TotalPrize = prizePerTicket * ticketsWon
                        };
                    })
                    .ToList();

                tierResults.Add(new TierResult { TierName = prizeLevel.Name, Winners = winners });
            }

            return new DrawResult
            {
                TotalRevenue = totalRevenue,
                HouseProfit = totalRevenue - totalPrizesAwarded,
                TierResults = tierResults
            };
        }

        private int DetermineWinnerCount(PrizeLevel prizeLevel, int totalTicketCount)
        {
            if (prizeLevel.FixedWinnerCount.HasValue)
                return prizeLevel.FixedWinnerCount.Value;

            if (prizeLevel.WinnerTicketRate.HasValue)
                return Math.Max(1, (int)Math.Floor(totalTicketCount * prizeLevel.WinnerTicketRate.Value));

            return 0;
        }

        private List<LotteryTicket> DrawTickets(List<LotteryTicket> pool, int count)
        {
            var drawn = new List<LotteryTicket>();

            for (int i = 0; i < count && pool.Count > 0; i++)
            {
                var index = _numberGenerator.Generate(0, pool.Count);
                drawn.Add(pool[index]);
                pool.RemoveAt(index);
            }

            return drawn;
        }
    }
}
