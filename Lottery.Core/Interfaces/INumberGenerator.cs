namespace Lottery.Core.Interfaces
{
    public interface INumberGenerator
    {
        int Generate(int minVal, int maxVal);
    }
}
