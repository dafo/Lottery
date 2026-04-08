using Lottery.Core.Interfaces;

namespace Lottery.Core.Services
{
    internal class NumberGenerator : INumberGenerator
    {
        public int Generate(int minVal, int maxVal)
        {
            return Random.Shared.Next(minVal, maxVal);
        }
    }
}
