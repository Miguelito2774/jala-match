using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MinorTeamTableUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "technical_weight",
                schema: "public",
                table: "teams",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "psychological_weight",
                schema: "public",
                table: "teams",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "minimum_sfia_level",
                schema: "public",
                table: "teams",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "interests_weight",
                schema: "public",
                table: "teams",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "experience_weight",
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
                name: "timezone_weight",
                schema: "public",
                table: "teams",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "sfia_level_general",
                schema: "public",
                table: "employee_profiles",
                type: "integer",
                precision: 3,
                scale: 1,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(3,1)",
                oldPrecision: 3,
                oldScale: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "experience_weight",
                schema: "public",
                table: "teams");

            migrationBuilder.DropColumn(
                name: "language_weight",
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
                name: "timezone_weight",
                schema: "public",
                table: "teams");

            migrationBuilder.AlterColumn<int>(
                name: "technical_weight",
                schema: "public",
                table: "teams",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "psychological_weight",
                schema: "public",
                table: "teams",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "minimum_sfia_level",
                schema: "public",
                table: "teams",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "interests_weight",
                schema: "public",
                table: "teams",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<decimal>(
                name: "sfia_level_general",
                schema: "public",
                table: "employee_profiles",
                type: "numeric(3,1)",
                precision: 3,
                scale: 1,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldPrecision: 3,
                oldScale: 1);
        }
    }
}
