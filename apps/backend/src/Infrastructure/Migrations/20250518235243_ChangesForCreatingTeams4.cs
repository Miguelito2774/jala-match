using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangesForCreatingTeams4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_team_required_technologies_technologies_technology_id",
                schema: "public",
                table: "team_required_technologies");

            migrationBuilder.DropColumn(
                name: "required_technologies",
                schema: "public",
                table: "teams");

            migrationBuilder.AddForeignKey(
                name: "fk_team_required_technologies_technologies_technology_id",
                schema: "public",
                table: "team_required_technologies",
                column: "technology_id",
                principalSchema: "public",
                principalTable: "technologies",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_team_required_technologies_technologies_technology_id",
                schema: "public",
                table: "team_required_technologies");

            migrationBuilder.AddColumn<string>(
                name: "required_technologies",
                schema: "public",
                table: "teams",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "fk_team_required_technologies_technologies_technology_id",
                schema: "public",
                table: "team_required_technologies",
                column: "technology_id",
                principalSchema: "public",
                principalTable: "technologies",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
