using Microsoft.EntityFrameworkCore;
using PokeDex.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
