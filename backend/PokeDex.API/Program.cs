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
builder.Services.AddScoped<ITeamRepository, TeamRepository>();

// Application Services
builder.Services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();
builder.Services.AddScoped<ITeamService, TeamService>();

// Configure CORS for Angular frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Angular dev server
              .AllowAnyHeader()                      // Allow any HTTP headers
              .AllowAnyMethod()                      // Allow GET, POST, PUT, DELETE, etc.
              .AllowCredentials();                   // Allow cookies/auth headers
    });
});

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

// Zero-touch startup: apply migrations and seed an empty database so a fresh
// clone works with `dotnet run` alone. Failures are logged but never block startup.
using (var scope = app.Services.CreateScope())
{
    var startupLogger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<PokedexDbContext>();
        await dbContext.Database.MigrateAsync();

        if (!await dbContext.Pokemon.AnyAsync())
        {
            startupLogger.LogInformation("Pokemon table is empty - seeding from PokeAPI...");
            var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
            var seedResult = await seeder.SeedDatabaseAsync();
            startupLogger.LogInformation(
                "Startup seeding finished: {Added} added, {Errors} errors",
                seedResult.PokemonAdded, seedResult.Errors.Count);
        }
    }
    catch (Exception ex)
    {
        startupLogger.LogError(ex,
            "Automatic migration/seeding failed. The API will start, but the database may be empty. " +
            "You can retry via POST /api/admin/seed");
    }
}

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

// Enable CORS - MUST come before UseAuthorization
app.UseCors("AllowAngularDev");

app.UseAuthorization();
app.MapControllers();
app.Run();
