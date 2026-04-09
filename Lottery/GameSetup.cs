using Lottery.Core.Factories;
using Lottery.Core.Interfaces;
using Lottery.Core.Models;
using Lottery.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lottery
{
    public static class GameSetup
    {
        public static LotterySettings LoadSettings()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            return config.GetSection("LotterySettings").Get<LotterySettings>()
                ?? throw new InvalidOperationException("LotterySettings section is missing from appsettings.json.");
        }

        public static ServiceProvider BuildServiceProvider(LotterySettings settings)
        {
            return new ServiceCollection()
                .AddSingleton(settings)
                .AddSingleton<INumberGenerator, NumberGenerator>()
                .AddTransient<ITicketService, TicketService>()
                .AddTransient<IPlayerFactory, PlayerFactory>()
                .AddTransient<ILotteryService, LotteryService>()
                .BuildServiceProvider();
        }
    }
}
