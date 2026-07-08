# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Restore first so layer caching kicks in when only source changes
COPY PokeDex.sln .
COPY PokeDex.Core/PokeDex.Core.csproj PokeDex.Core/
COPY PokeDex.Data/PokeDex.Data.csproj PokeDex.Data/
COPY PokeDex.Application/PokeDex.Application.csproj PokeDex.Application/
COPY PokeDex.API/PokeDex.API.csproj PokeDex.API/
RUN dotnet restore PokeDex.API/PokeDex.API.csproj

COPY . .
RUN dotnet publish PokeDex.API/PokeDex.API.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "PokeDex.API.dll"]
