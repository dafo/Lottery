using Lottery.Core.Models;

namespace Lottery.Tests
{
    public class GameSetupTests
    {
        [Fact]
        public void ValidateSettings_RevenueRatesWithinLimit_DoesNotThrow()
        {
            var settings = new LotterySettings
            {
                PrizeLevels = new List<PrizeLevel>
                  {
                      new() { Name = "Grand Prize", RevenueRate = 0.50m },
                      new() { Name = "Second Tier", RevenueRate = 0.30m },
                      new() { Name = "Third Tier", RevenueRate = 0.10m }
                  }
            };

            var exception = Record.Exception(() => GameSetup.ValidateSettings(settings));
            Assert.Null(exception);
        }

        [Fact]
        public void ValidateSettings_RevenueRatesAtExactly100Percent_DoesNotThrow()
        {
            var settings = new LotterySettings
            {
                PrizeLevels = new List<PrizeLevel>
                  {
                      new() { Name = "A", RevenueRate = 0.60m },
                      new() { Name = "B", RevenueRate = 0.40m }
                  }
            };

            var exception = Record.Exception(() => GameSetup.ValidateSettings(settings));
            Assert.Null(exception);
        }

        [Fact]
        public void ValidateSettings_RevenueRatesExceed100Percent_Throws()
        {
            var settings = new LotterySettings
            {
                PrizeLevels = new List<PrizeLevel>
                  {
                      new() { Name = "A", RevenueRate = 0.60m },
                      new() { Name = "B", RevenueRate = 0.50m }
                  }
            };

            Assert.Throws<InvalidOperationException>(() => GameSetup.ValidateSettings(settings));
        }
    }
}
