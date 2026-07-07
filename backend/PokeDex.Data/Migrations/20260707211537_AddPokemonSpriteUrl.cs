using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokeDex.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPokemonSpriteUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SpriteUrl",
                table: "Pokemon",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpriteUrl",
                table: "Pokemon");
        }
    }
}
