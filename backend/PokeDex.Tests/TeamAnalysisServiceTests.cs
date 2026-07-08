using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using PokeDex.Application.Services;
using PokeDex.Core.Models;
using PokeDex.Data;
using PokeDex.Data.Repositories;

namespace PokeDex.Tests
{
    public class TeamAnalysisServiceTests
    {
        private const string Ash = "user-ash";

        // Type IDs from the HasData seed: Fire=2, Water=3, Flying=10
        private static (TeamAnalysisService Analysis, TeamService Teams) CreateServices()
        {
            var options = new DbContextOptionsBuilder<PokedexDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new PokedexDbContext(options);
            context.Database.EnsureCreated();

            context.Pokemon.Add(new Pokemon
            {
                Id = 1, PokedexNumber = 6, Name = "charizard",
                HP = 78, Attack = 84, Defense = 78,
                SpecialAttack = 109, SpecialDefense = 85, Speed = 100,
                PrimaryTypeId = 2, SecondaryTypeId = 10 // Fire/Flying
            });
            context.Pokemon.Add(new Pokemon
            {
                Id = 2, PokedexNumber = 9, Name = "blastoise",
                HP = 79, Attack = 83, Defense = 100,
                SpecialAttack = 85, SpecialDefense = 105, Speed = 78,
                PrimaryTypeId = 3 // Water
            });
            context.SaveChanges();

            var teams = new TeamService(
                new TeamRepository(context),
                new PokemonRepository(context),
                NullLogger<TeamService>.Instance);

            return (new TeamAnalysisService(teams), teams);
        }

        private static async Task<int> CreateTeamWithBoth(TeamService teams)
        {
            var created = await teams.CreateTeamAsync("Kanto", Ash);
            await teams.AddPokemonAsync(created.Team!.Id, 1, Ash);
            await teams.AddPokemonAsync(created.Team.Id, 2, Ash);
            return created.Team.Id;
        }

        [Fact]
        public async Task Analyze_ComputesDefensiveMultipliers()
        {
            var (analysisService, teams) = CreateServices();
            var teamId = await CreateTeamWithBoth(teams);

            var analysis = await analysisService.AnalyzeAsync(teamId, Ash);

            Assert.NotNull(analysis);
            var rock = analysis.Defense.Single(d => d.Type == "Rock");
            Assert.Equal(1, rock.Weak); // Charizard 4x, Blastoise neutral
            Assert.Equal(4.0, rock.Members.Single(m => m.PokemonName == "charizard").Multiplier);

            var fire = analysis.Defense.Single(d => d.Type == "Fire");
            Assert.Equal(2, fire.Resist); // both resist Fire

            var ground = analysis.Defense.Single(d => d.Type == "Ground");
            Assert.Equal(1, ground.Immune); // Charizard is airborne
        }

        [Fact]
        public async Task Analyze_FlagsThreatsAndUnresistedTypes()
        {
            var (analysisService, teams) = CreateServices();
            var teamId = await CreateTeamWithBoth(teams);

            var analysis = await analysisService.AnalyzeAsync(teamId, Ash);

            // Both members are weak to Electric and neither resists it
            Assert.Contains("Electric", analysis!.Threats);
            Assert.Contains("Electric", analysis.Unresisted);
            // Fire is resisted by both, so it's neither
            Assert.DoesNotContain("Fire", analysis.Threats);
            Assert.DoesNotContain("Fire", analysis.Unresisted);
        }

        [Fact]
        public async Task Analyze_BuildsStatSummaries()
        {
            var (analysisService, teams) = CreateServices();
            var teamId = await CreateTeamWithBoth(teams);

            var analysis = await analysisService.AnalyzeAsync(teamId, Ash);

            var speed = analysis!.Stats.Single(s => s.Stat == "Speed");
            Assert.Equal(89.0, speed.Average); // (100 + 78) / 2
            Assert.Equal("charizard", speed.BestName);
            Assert.Equal(100, speed.BestValue);
        }

        [Fact]
        public async Task Analyze_EmptyTeam_HasNoThreatsOrStats()
        {
            var (analysisService, teams) = CreateServices();
            var created = await teams.CreateTeamAsync("Empty", Ash);

            var analysis = await analysisService.AnalyzeAsync(created.Team!.Id, Ash);

            Assert.NotNull(analysis);
            Assert.Equal(18, analysis.Defense.Count);
            Assert.Empty(analysis.Threats);
            Assert.Empty(analysis.Unresisted);
            Assert.Empty(analysis.Stats);
        }

        [Fact]
        public async Task Analyze_OtherUsersTeam_ReturnsNull()
        {
            var (analysisService, teams) = CreateServices();
            var teamId = await CreateTeamWithBoth(teams);

            var analysis = await analysisService.AnalyzeAsync(teamId, "user-misty");

            Assert.Null(analysis);
        }
    }
}
