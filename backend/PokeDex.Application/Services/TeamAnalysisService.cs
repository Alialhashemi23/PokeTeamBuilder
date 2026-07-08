using PokeDex.Core.Models;
using PokeDex.Core.Services;

namespace PokeDex.Application.Services
{
    public class TeamAnalysisService : ITeamAnalysisService
    {
        private readonly ITeamService _teamService;

        public TeamAnalysisService(ITeamService teamService)
        {
            _teamService = teamService;
        }

        public async Task<TeamAnalysis?> AnalyzeAsync(int teamId, string ownerId)
        {
            var team = await _teamService.GetTeamByIdAsync(teamId, ownerId);
            if (team == null)
            {
                return null;
            }

            var members = team.TeamPokemon.OrderBy(tp => tp.Id).ToList();
            var analysis = new TeamAnalysis();

            foreach (var attackingType in TypeChart.Types)
            {
                var matchup = new TypeMatchup { Type = attackingType };

                foreach (var member in members)
                {
                    var multiplier = TypeChart.GetDefensiveMultiplier(
                        attackingType,
                        member.Pokemon.PrimaryType.Name,
                        member.Pokemon.SecondaryType?.Name);

                    matchup.Members.Add(new MemberMultiplier
                    {
                        TeamPokemonId = member.Id,
                        PokemonName = member.Pokemon.Name,
                        Multiplier = multiplier
                    });

                    if (multiplier == 0)
                    {
                        matchup.Immune++;
                    }
                    else if (multiplier < 1)
                    {
                        matchup.Resist++;
                    }
                    else if (multiplier > 1)
                    {
                        matchup.Weak++;
                    }
                }

                analysis.Defense.Add(matchup);

                if (members.Count > 0)
                {
                    var covered = matchup.Resist + matchup.Immune;
                    if (matchup.Weak > covered)
                    {
                        analysis.Threats.Add(attackingType);
                    }
                    if (covered == 0)
                    {
                        analysis.Unresisted.Add(attackingType);
                    }
                }
            }

            if (members.Count > 0)
            {
                analysis.Stats = BuildStatSummaries(members);
            }

            return analysis;
        }

        private static List<StatSummary> BuildStatSummaries(List<TeamPokemon> members)
        {
            var stats = new (string Name, Func<Pokemon, int> Get)[]
            {
                ("HP", p => p.HP),
                ("Attack", p => p.Attack),
                ("Defense", p => p.Defense),
                ("Sp. Atk", p => p.SpecialAttack),
                ("Sp. Def", p => p.SpecialDefense),
                ("Speed", p => p.Speed)
            };

            return stats.Select(stat =>
            {
                var best = members.MaxBy(m => stat.Get(m.Pokemon))!.Pokemon;
                return new StatSummary
                {
                    Stat = stat.Name,
                    Average = Math.Round(members.Average(m => stat.Get(m.Pokemon)), 1),
                    BestName = best.Name,
                    BestValue = stat.Get(best)
                };
            }).ToList();
        }
    }
}
