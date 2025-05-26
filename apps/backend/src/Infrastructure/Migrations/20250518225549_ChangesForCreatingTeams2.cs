using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangesForCreatingTeams2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "minimum_sfia_level",
                schema: "public",
                table: "teams");

            migrationBuilder.DropColumn(
                name: "required_roles",
                schema: "public",
                table: "teams");

            migrationBuilder.DropColumn(
                name: "required_technologies_json",
                schema: "public",
                table: "teams");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "minimum_sfia_level",
                schema: "public",
                table: "teams",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "required_roles",
                schema: "public",
                table: "teams",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "required_technologies_json",
                schema: "public",
                table: "teams",
                type: "text",
                nullable: true);
        }
    }
}
