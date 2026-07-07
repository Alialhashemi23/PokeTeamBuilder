using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using PokeDex.Application.Services;
using PokeDex.Core.Models.DTOs;
using PokeDex.Core.Services;
using PokeDex.Data;
using PokeDex.Data.Repositories;

namespace PokeDex.Tests
{
    public class DatabaseSeederTests
    {
        private class FakePokeApiService : IPokeApiService
        {
            public List<PokemonApiDto> Pokemon { get; set; } = new();

            public Task<List<PokemonApiDto>> FetchPokemonAsync(int count = 30)
            {
                return Task.FromResult(Pokemon);
            }
        }

        private static PokemonApiDto Bulbasaur(string? spriteUrl = null) => new()
        {
            PokedexNumber = 1,
            Name = "bulbasaur",
            HP = 45, Attack = 49, Defense = 49,
            SpecialAttack = 65, SpecialDefense = 65, Speed = 45,
            PrimaryType = "grass",
            SecondaryType = "poison",
            SpriteUrl = spriteUrl
        };

        private static (DatabaseSeeder Seeder, PokedexDbContext Context, FakePokeApiService Api) CreateSeeder()
        {
            var options = new DbContextOptionsBuilder<PokedexDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new PokedexDbContext(options);
            context.Database.EnsureCreated(); // applies HasData type seeding

            var api = new FakePokeApiService();
            var seeder = new DatabaseSeeder(
                api,
                new PokemonRepository(context),
                context,
                NullLogger<DatabaseSeeder>.Instance);

            return (seeder, context, api);
        }

        [Fact]
        public async Task Seed_AddsNewPokemonWithSpriteAndResolvedTypes()
        {
            var (seeder, context, api) = CreateSeeder();
            api.Pokemon.Add(Bulbasaur("https://sprites/1.png"));

            var result = await seeder.SeedDatabaseAsync();

            Assert.Equal(1, result.PokemonAdded);
            Assert.Empty(result.Errors);
            var saved = Assert.Single(context.Pokemon);
            Assert.Equal("https://sprites/1.png", saved.SpriteUrl);
            Assert.Equal("Grass", context.PokemonTypes.Single(t => t.Id == saved.PrimaryTypeId).Name);
            Assert.Equal("Poison", context.PokemonTypes.Single(t => t.Id == saved.SecondaryTypeId).Name);
        }

        [Fact]
        public async Task Seed_BackfillsSpriteOnExistingPokemon()
        {
            var (seeder, context, api) = CreateSeeder();
            api.Pokemon.Add(Bulbasaur(spriteUrl: null));
            await seeder.SeedDatabaseAsync(); // first seed, before sprites existed

            api.Pokemon[0] = Bulbasaur("https://sprites/1.png");
            var result = await seeder.SeedDatabaseAsync();

            Assert.Equal(0, result.PokemonAdded);
            Assert.Equal(1, result.PokemonUpdated);
            Assert.Equal("https://sprites/1.png", context.Pokemon.Single().SpriteUrl);
        }

        [Fact]
        public async Task Seed_SkipsUnchangedPokemon()
        {
            var (seeder, _, api) = CreateSeeder();
            api.Pokemon.Add(Bulbasaur("https://sprites/1.png"));
            await seeder.SeedDatabaseAsync();

            var result = await seeder.SeedDatabaseAsync();

            Assert.Equal(0, result.PokemonAdded);
            Assert.Equal(0, result.PokemonUpdated);
            Assert.Equal(1, result.PokemonSkipped);
        }

        [Fact]
        public async Task Seed_SkipsPokemonWithUnknownPrimaryType()
        {
            var (seeder, context, api) = CreateSeeder();
            var dto = Bulbasaur();
            dto.PrimaryType = "not-a-type";
            api.Pokemon.Add(dto);

            var result = await seeder.SeedDatabaseAsync();

            Assert.Equal(0, result.PokemonAdded);
            Assert.Equal(1, result.PokemonSkipped);
            Assert.Empty(context.Pokemon);
        }
    }
}
