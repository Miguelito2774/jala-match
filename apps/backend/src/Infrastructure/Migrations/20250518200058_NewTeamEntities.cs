using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewTeamEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_team_members_employee_profiles_employee_profile_id",
                schema: "public",
                table: "team_members");

            migrationBuilder.DropColumn(
                name: "joined_date",
                schema: "public",
                table: "team_members");

            migrationBuilder.AddColumn<bool>(
                name: "is_leader",
                schema: "public",
                table: "team_members",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "name",
                schema: "public",
                table: "team_members",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "sfia_level",
                schema: "public",
                table: "team_members",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "recommended_members",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    compatibility_score = table.Column<int>(type: "integer", nullable: false),
                    analysis = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_recommended_members", x => x.id);
                });

            migrationBuilder.AddForeignKey(
                name: "fk_team_members_employee_profiles_employee_profile_id",
                schema: "public",
                table: "team_members",
                column: "employee_profile_id",
                principalSchema: "public",
                principalTable: "employee_profiles",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_team_members_employee_profiles_employee_profile_id",
                schema: "public",
                table: "team_members");

            migrationBuilder.DropTable(
                name: "recommended_members",
                schema: "public");

            migrationBuilder.DropColumn(
                name: "is_leader",
                schema: "public",
                table: "team_members");

            migrationBuilder.DropColumn(
                name: "name",
                schema: "public",
                table: "team_members");

            migrationBuilder.DropColumn(
                name: "sfia_level",
                schema: "public",
                table: "team_members");

            migrationBuilder.AddColumn<DateTime>(
                name: "joined_date",
                schema: "public",
                table: "team_members",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "fk_team_members_employee_profiles_employee_profile_id",
                schema: "public",
                table: "team_members",
                column: "employee_profile_id",
                principalSchema: "public",
                principalTable: "employee_profiles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
