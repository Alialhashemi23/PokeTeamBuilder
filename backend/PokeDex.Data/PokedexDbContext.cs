using Microsoft.EntityFrameworkCore;
using PokeDex.Core.Models;

namespace PokeDex.Data
{
    public class PokedexDbContext : DbContext
    {
        public PokedexDbContext(DbContextOptions<PokedexDbContext> options) : base(options)
        {
        }

        public DbSet<Pokemon> Pokemon { get; set; }
        public DbSet<PokemonType> PokemonTypes { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamPokemon> TeamPokemon { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Pokemon>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(50);
                entity.Property(p => p.PokedexNumber).IsRequired();

                entity.HasOne(p => p.PrimaryType)
                    .WithMany()
                    .HasForeignKey(p => p.PrimaryTypeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.SecondaryType)
                    .WithMany()
                    .HasForeignKey(p => p.SecondaryTypeId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);
            });

            modelBuilder.Entity<PokemonType>(entity =>
            {
                entity.HasKey(pt => pt.Id);
                entity.Property(pt => pt.Name).IsRequired().HasMaxLength(30);

                entity.HasData(
                    new PokemonType { Id = 1, Name = "Normal" },
                    new PokemonType { Id = 2, Name = "Fire" },
                    new PokemonType { Id = 3, Name = "Water" },
                    new PokemonType { Id = 4, Name = "Electric" },
                    new PokemonType { Id = 5, Name = "Grass" },
                    new PokemonType { Id = 6, Name = "Ice" },
                    new PokemonType { Id = 7, Name = "Fighting" },
                    new PokemonType { Id = 8, Name = "Poison" },
                    new PokemonType { Id = 9, Name = "Ground" },
                    new PokemonType { Id = 10, Name = "Flying" },
                    new PokemonType { Id = 11, Name = "Psychic" },
                    new PokemonType { Id = 12, Name = "Bug" },
                    new PokemonType { Id = 13, Name = "Rock" },
                    new PokemonType { Id = 14, Name = "Ghost" },
                    new PokemonType { Id = 15, Name = "Dragon" },
                    new PokemonType { Id = 16, Name = "Dark" },
                    new PokemonType { Id = 17, Name = "Steel" },
                    new PokemonType { Id = 18, Name = "Fairy" }
                );
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Name).IsRequired().HasMaxLength(50);
            });

            modelBuilder.Entity<TeamPokemon>(entity =>
            {
                entity.HasKey(tp => tp.Id);

                entity.HasOne(tp => tp.Team)
                    .WithMany(t => t.TeamPokemon)
                    .HasForeignKey(tp => tp.TeamId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(tp => tp.Pokemon)
                    .WithMany()
                    .HasForeignKey(tp => tp.PokemonId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
