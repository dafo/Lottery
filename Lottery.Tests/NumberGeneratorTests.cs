using Lottery.Core.Services;

namespace Lottery.Tests
{
    public class NumberGeneratorTests
    {
        [Fact]
        public void Generate_ReturnsValueWithinRange()
        {
            var generator = new NumberGenerator();

            var results = Enumerable.Range(0, 100)
                .Select(_ => generator.Generate(1, 10))
                .ToList();

            Assert.All(results, r =>
            {
                Assert.True(r >= 1, $"Value {r} was below minimum 1");
                Assert.True(r < 10, $"Value {r} was at or above maximum 10");
            });
        }

        [Fact]
        public void Generate_WithConsecutiveMinMax_AlwaysReturnsMin()
        {
            var generator = new NumberGenerator();

            var result = generator.Generate(5, 6);

            Assert.Equal(5, result);
        }

        [Fact]
        public void Generate_ProducesVariedResults()
        {
            var generator = new NumberGenerator();

            var results = Enumerable.Range(0, 50)
                .Select(_ => generator.Generate(1, 100))
                .ToHashSet();

            Assert.True(results.Count > 1, "Expected varied results from random generator");
        }
    }
}
