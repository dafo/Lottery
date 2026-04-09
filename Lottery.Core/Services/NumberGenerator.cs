using Lottery.Core.Interfaces;

namespace Lottery.Core.Services
{
    public class NumberGenerator : INumberGenerator
    {
        public int Generate(int minVal, int maxVal)
        {
            return Random.Shared.Next(minVal, maxVal);
        }
    }
}
