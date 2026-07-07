namespace PokeDex.API.DTOs
{
    /// <summary>
    /// Data Transfer Object for a team and its members
    /// </summary>
    public class TeamDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public List<TeamMemberDto> Members { get; set; } = new();
    }

    /// <summary>
    /// A single slot on a team: the join-row ID (used to remove the member) plus the Pokemon itself
    /// </summary>
    public class TeamMemberDto
    {
        public int TeamPokemonId { get; set; }
        public PokemonDto Pokemon { get; set; } = null!;
    }

    public class CreateTeamRequest
    {
        public string Name { get; set; } = string.Empty;
    }

    public class RenameTeamRequest
    {
        public string Name { get; set; } = string.Empty;
    }

    public class AddTeamMemberRequest
    {
        public int PokemonId { get; set; }
    }
}
