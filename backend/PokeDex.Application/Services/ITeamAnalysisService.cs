namespace PokeDex.Application.Services
{
    public interface ITeamAnalysisService
    {
        /// <summary>
        /// Type coverage and stat analysis for a team. Null when the team
        /// doesn't exist or isn't owned by the caller.
        /// </summary>
        Task<TeamAnalysis?> AnalyzeAsync(int teamId, string ownerId);
    }

    public class TeamAnalysis
    {
        /// <summary>One entry per attacking type, with the team's defensive response</summary>
        public List<TypeMatchup> Defense { get; set; } = new();

        /// <summary>Types more of the team is weak to than resists</summary>
        public List<string> Threats { get; set; } = new();

        /// <summary>Types nobody on the team resists or is immune to</summary>
        public List<string> Unresisted { get; set; } = new();

        public List<StatSummary> Stats { get; set; } = new();
    }

    public class TypeMatchup
    {
        public string Type { get; set; } = string.Empty;
        public int Weak { get; set; }
        public int Resist { get; set; }
        public int Immune { get; set; }
        public List<MemberMultiplier> Members { get; set; } = new();
    }

    public class MemberMultiplier
    {
        public int TeamPokemonId { get; set; }
        public string PokemonName { get; set; } = string.Empty;
        public double Multiplier { get; set; }
    }

    public class StatSummary
    {
        public string Stat { get; set; } = string.Empty;
        public double Average { get; set; }
        public string BestName { get; set; } = string.Empty;
        public int BestValue { get; set; }
    }
}
