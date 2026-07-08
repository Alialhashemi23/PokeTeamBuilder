using PokeDex.Core.Services;

namespace PokeDex.Tests
{
    public class TypeChartTests
    {
        [Theory]
        // Super effective
        [InlineData("Water", "Fire", 2.0)]
        [InlineData("Electric", "Flying", 2.0)]
        [InlineData("Ice", "Dragon", 2.0)]
        [InlineData("Fairy", "Dark", 2.0)]
        // Not very effective
        [InlineData("Fire", "Water", 0.5)]
        [InlineData("Grass", "Steel", 0.5)]
        [InlineData("Dark", "Fairy", 0.5)]
        // Immunities
        [InlineData("Normal", "Ghost", 0.0)]
        [InlineData("Electric", "Ground", 0.0)]
        [InlineData("Ground", "Flying", 0.0)]
        [InlineData("Poison", "Steel", 0.0)]
        [InlineData("Psychic", "Dark", 0.0)]
        [InlineData("Dragon", "Fairy", 0.0)]
        // Neutral
        [InlineData("Normal", "Normal", 1.0)]
        [InlineData("Fire", "Electric", 1.0)]
        public void GetMultiplier_MatchesKnownChart(string attacking, string defending, double expected)
        {
            Assert.Equal(expected, TypeChart.GetMultiplier(attacking, defending));
        }

        [Fact]
        public void GetMultiplier_IsCaseInsensitive()
        {
            Assert.Equal(2.0, TypeChart.GetMultiplier("water", "FIRE"));
        }

        [Theory]
        // Fire/Flying (Charizard): Rock 2x2=4, Water 2, Grass 0.5x0.5=0.25, Ground 2x0=0
        [InlineData("Rock", "Fire", "Flying", 4.0)]
        [InlineData("Water", "Fire", "Flying", 2.0)]
        [InlineData("Grass", "Fire", "Flying", 0.25)]
        [InlineData("Ground", "Fire", "Flying", 0.0)]
        // Single type passes through
        [InlineData("Water", "Fire", null, 2.0)]
        public void GetDefensiveMultiplier_CombinesDualTypes(
            string attacking, string primary, string? secondary, double expected)
        {
            Assert.Equal(expected, TypeChart.GetDefensiveMultiplier(attacking, primary, secondary));
        }

        [Fact]
        public void Chart_Covers18Types()
        {
            Assert.Equal(18, TypeChart.Types.Count);
        }
    }
}
