using System.Reflection;
using Microsoft.EntityFrameworkCore;
using PokeDex.Application.Services;
using PokeDex.Core.Services;
using PokeDex.Data;
using PokeDex.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PokedexDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// External API Services
builder.Services.AddHttpClient<IPokeApiService, PokeApiService>();

// Repository Layer
builder.Services.AddScoped<IPokemonRepository, PokemonRepository>();

// Application Services
builder.Services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();

builder.Services.AddControllers();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "PokeDex API",
        Version = "v1",
        Description = "A Pokedex API that fetches Pokemon data from PokeAPI and manages teams"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "PokeDex API v1");
        options.RoutePrefix = string.Empty; // Swagger UI at root (http://localhost:5000/)
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
