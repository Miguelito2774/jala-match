using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangesForCreatingTeams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "description",
                schema: "public",
                table: "teams");

            migrationBuilder.DropColumn(
                name: "experience_weight",
                schema: "public",
                table: "teams");

            migrationBuilder.DropColumn(
                name: "interests_weight",
                schema: "public",
                table: "teams");

            migrationBuilder.DropColumn(
                name: "language_weight",
                schema: "public",
                table: "teams");

            migrationBuilder.DropColumn(
                name: "psychological_weight",
                schema: "public",
                table: "teams");

            migrationBuilder.DropColumn(
                name: "sfia_weight",
                schema: "public",
                table: "teams");

            migrationBuilder.DropColumn(
                name: "team_size",
                schema: "public",
                table: "teams");

            migrationBuilder.DropColumn(
                name: "technical_weight",
                schema: "public",
                table: "teams");

            migrationBuilder.DropColumn(
                name: "timezone_weight",
                schema: "public",
                table: "teams");

            migrationBuilder.AlterColumn<string>(
                name: "members_json",
                schema: "public",
                table: "teams",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "required_technologies",
                schema: "public",
                table: "teams",
                type: "jsonb",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "required_technologies",
                schema: "public",
                table: "teams");

            migrationBuilder.AlterColumn<string>(
                name: "members_json",
                schema: "public",
                table: "teams",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "description",
                schema: "public",
                table: "teams",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "experience_weight",
                schema: "public",
                table: "teams",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "interests_weight",
                schema: "public",
                table: "teams",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "language_weight",
                schema: "public",
                table: "teams",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "psychological_weight",
                schema: "public",
                table: "teams",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "sfia_weight",
                schema: "public",
                table: "teams",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "team_size",
                schema: "public",
                table: "teams",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "technical_weight",
                schema: "public",
                table: "teams",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "timezone_weight",
                schema: "public",
                table: "teams",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
