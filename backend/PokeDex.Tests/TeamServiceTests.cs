using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using PokeDex.Application.Services;
using PokeDex.Core.Models;
using PokeDex.Data;
using PokeDex.Data.Repositories;

namespace PokeDex.Tests
{
    public class TeamServiceTests
    {
        /// <summary>
        /// Creates a TeamService backed by a fresh in-memory database seeded
        /// with the 18 Pokemon types and a handful of Pokemon.
        /// </summary>
        private static (TeamService Service, PokedexDbContext Context) CreateService()
        {
            var options = new DbContextOptionsBuilder<PokedexDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new PokedexDbContext(options);
            context.Database.EnsureCreated(); // applies HasData type seeding

            for (var i = 1; i <= 10; i++)
            {
                context.Pokemon.Add(new Pokemon
                {
                    PokedexNumber = i,
                    Name = $"pokemon-{i}",
                    HP = 50, Attack = 50, Defense = 50,
                    SpecialAttack = 50, SpecialDefense = 50, Speed = 50,
                    PrimaryTypeId = 1
                });
            }
            context.SaveChanges();

            var service = new TeamService(
                new TeamRepository(context),
                new PokemonRepository(context),
                NullLogger<TeamService>.Instance);

            return (service, context);
        }

        [Fact]
        public async Task CreateTeam_WithValidName_CreatesTeam()
        {
            var (service, _) = CreateService();

            var result = await service.CreateTeamAsync("  Kanto Squad  ");

            Assert.True(result.Success);
            Assert.NotNull(result.Team);
            Assert.Equal("Kanto Squad", result.Team.Name); // name is trimmed
            Assert.True(result.Team.Id > 0);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task CreateTeam_WithBlankName_Fails(string name)
        {
            var (service, _) = CreateService();

            var result = await service.CreateTeamAsync(name);

            Assert.False(result.Success);
            Assert.Equal(TeamError.InvalidName, result.Error);
        }

        [Fact]
        public async Task CreateTeam_WithNameOverMaxLength_Fails()
        {
            var (service, _) = CreateService();

            var result = await service.CreateTeamAsync(new string('x', TeamService.MaxNameLength + 1));

            Assert.False(result.Success);
            Assert.Equal(TeamError.InvalidName, result.Error);
        }

        [Fact]
        public async Task RenameTeam_UpdatesName()
        {
            var (service, _) = CreateService();
            var created = await service.CreateTeamAsync("Old Name");

            var result = await service.RenameTeamAsync(created.Team!.Id, "New Name");

            Assert.True(result.Success);
            Assert.Equal("New Name", result.Team!.Name);
        }

        [Fact]
        public async Task RenameTeam_WhenTeamMissing_ReturnsNotFound()
        {
            var (service, _) = CreateService();

            var result = await service.RenameTeamAsync(999, "New Name");

            Assert.False(result.Success);
            Assert.Equal(TeamError.TeamNotFound, result.Error);
        }

        [Fact]
        public async Task DeleteTeam_RemovesTeam()
        {
            var (service, context) = CreateService();
            var created = await service.CreateTeamAsync("Doomed");

            var result = await service.DeleteTeamAsync(created.Team!.Id);

            Assert.True(result.Success);
            Assert.Empty(context.Teams);
        }

        [Fact]
        public async Task AddPokemon_AddsMemberWithNavigationLoaded()
        {
            var (service, _) = CreateService();
            var created = await service.CreateTeamAsync("Squad");

            var result = await service.AddPokemonAsync(created.Team!.Id, 1);

            Assert.True(result.Success);
            var member = Assert.Single(result.Team!.TeamPokemon);
            Assert.Equal(1, member.PokemonId);
            Assert.NotNull(member.Pokemon);
            Assert.Equal("pokemon-1", member.Pokemon.Name);
        }

        [Fact]
        public async Task AddPokemon_WhenPokemonMissing_ReturnsNotFound()
        {
            var (service, _) = CreateService();
            var created = await service.CreateTeamAsync("Squad");

            var result = await service.AddPokemonAsync(created.Team!.Id, 999);

            Assert.False(result.Success);
            Assert.Equal(TeamError.PokemonNotFound, result.Error);
        }

        [Fact]
        public async Task AddPokemon_WhenAlreadyOnTeam_ReturnsDuplicate()
        {
            var (service, _) = CreateService();
            var created = await service.CreateTeamAsync("Squad");
            await service.AddPokemonAsync(created.Team!.Id, 1);

            var result = await service.AddPokemonAsync(created.Team.Id, 1);

            Assert.False(result.Success);
            Assert.Equal(TeamError.DuplicatePokemon, result.Error);
        }

        [Fact]
        public async Task AddPokemon_WhenTeamFull_ReturnsTeamFull()
        {
            var (service, _) = CreateService();
            var created = await service.CreateTeamAsync("Squad");
            for (var pokemonId = 1; pokemonId <= TeamService.MaxTeamSize; pokemonId++)
            {
                var added = await service.AddPokemonAsync(created.Team!.Id, pokemonId);
                Assert.True(added.Success);
            }

            var result = await service.AddPokemonAsync(created.Team!.Id, TeamService.MaxTeamSize + 1);

            Assert.False(result.Success);
            Assert.Equal(TeamError.TeamFull, result.Error);
        }

        [Fact]
        public async Task RemovePokemon_RemovesOnlyThatMember()
        {
            var (service, _) = CreateService();
            var created = await service.CreateTeamAsync("Squad");
            await service.AddPokemonAsync(created.Team!.Id, 1);
            var afterSecondAdd = await service.AddPokemonAsync(created.Team.Id, 2);
            var memberToRemove = afterSecondAdd.Team!.TeamPokemon.First(tp => tp.PokemonId == 1);

            var result = await service.RemovePokemonAsync(created.Team.Id, memberToRemove.Id);

            Assert.True(result.Success);
            var remaining = Assert.Single(result.Team!.TeamPokemon);
            Assert.Equal(2, remaining.PokemonId);
        }

        [Fact]
        public async Task RemovePokemon_WhenMemberMissing_ReturnsNotFound()
        {
            var (service, _) = CreateService();
            var created = await service.CreateTeamAsync("Squad");

            var result = await service.RemovePokemonAsync(created.Team!.Id, 999);

            Assert.False(result.Success);
            Assert.Equal(TeamError.MemberNotFound, result.Error);
        }

        [Fact]
        public async Task GetAllTeams_ReturnsTeamsWithMembers()
        {
            var (service, _) = CreateService();
            var created = await service.CreateTeamAsync("Squad");
            await service.AddPokemonAsync(created.Team!.Id, 3);

            var teams = await service.GetAllTeamsAsync();

            var team = Assert.Single(teams);
            var member = Assert.Single(team.TeamPokemon);
            Assert.Equal("pokemon-3", member.Pokemon.Name);
        }
    }
}
