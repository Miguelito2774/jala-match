using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewDbSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "members",
                schema: "public",
                table: "teams");

            migrationBuilder.DropColumn(
                name: "required_technologies",
                schema: "public",
                table: "teams");

            migrationBuilder.DropColumn(
                name: "personal_interests",
                schema: "public",
                table: "employee_profiles");

            migrationBuilder.DropColumn(
                name: "work_experience",
                schema: "public",
                table: "employee_profiles");

            migrationBuilder.AddColumn<string>(
                name: "description",
                schema: "public",
                table: "technology_categories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "description",
                schema: "public",
                table: "technologies",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ai_analysis",
                schema: "public",
                table: "teams",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "jsonb");

            migrationBuilder.AddColumn<string>(
                name: "description",
                schema: "public",
                table: "teams",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "interests_weight",
                schema: "public",
                table: "teams",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "members_json",
                schema: "public",
                table: "teams",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "minimum_sfia_level",
                schema: "public",
                table: "teams",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "psychological_weight",
                schema: "public",
                table: "teams",
                type: "integer",
                nullable: true);

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

            migrationBuilder.AddColumn<int>(
                name: "technical_weight",
                schema: "public",
                table: "teams",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "weight_criteria",
                schema: "public",
                table: "teams",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "experience_level",
                schema: "public",
                table: "employee_technologies",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "version",
                schema: "public",
                table: "employee_technologies",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "timezone",
                schema: "public",
                table: "employee_profiles",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "specialization",
                schema: "public",
                table: "employee_profiles",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "sfia_level_general",
                schema: "public",
                table: "employee_profiles",
                type: "numeric(3,1)",
                precision: 3,
                scale: 1,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "mbti",
                schema: "public",
                table: "employee_profiles",
                type: "character varying(4)",
                maxLength: 4,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(4)",
                oldMaxLength: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "country",
                schema: "public",
                table: "employee_profiles",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "employee_languages",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    employee_profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    language = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    proficiency = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_employee_languages", x => x.id);
                    table.ForeignKey(
                        name: "fk_employee_languages_employee_profiles_employee_profile_id",
                        column: x => x.employee_profile_id,
                        principalSchema: "public",
                        principalTable: "employee_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "personal_interests",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    employee_profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    session_duration_minutes = table.Column<int>(type: "integer", nullable: true),
                    frequency = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    interest_level = table.Column<int>(type: "integer", nullable: true, defaultValue: 3)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_personal_interests", x => x.id);
                    table.ForeignKey(
                        name: "fk_personal_interests_employee_profiles_employee_profile_id",
                        column: x => x.employee_profile_id,
                        principalSchema: "public",
                        principalTable: "employee_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "team_members",
                schema: "public",
                columns: table => new
                {
                    team_id = table.Column<Guid>(type: "uuid", nullable: false),
                    employee_profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    joined_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_team_members", x => new { x.team_id, x.employee_profile_id });
                    table.ForeignKey(
                        name: "fk_team_members_employee_profiles_employee_profile_id",
                        column: x => x.employee_profile_id,
                        principalSchema: "public",
                        principalTable: "employee_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_team_members_teams_team_id",
                        column: x => x.team_id,
                        principalSchema: "public",
                        principalTable: "teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "team_required_technologies",
                schema: "public",
                columns: table => new
                {
                    team_id = table.Column<Guid>(type: "uuid", nullable: false),
                    technology_id = table.Column<Guid>(type: "uuid", nullable: false),
                    minimum_sfia_level = table.Column<int>(type: "integer", nullable: false, defaultValue: 3),
                    is_mandatory = table.Column<bool>(type: "boolean", nullable: false),
                    id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_team_required_technologies", x => new { x.team_id, x.technology_id });
                    table.ForeignKey(
                        name: "fk_team_required_technologies_teams_team_id",
                        column: x => x.team_id,
                        principalSchema: "public",
                        principalTable: "teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_team_required_technologies_technologies_technology_id",
                        column: x => x.technology_id,
                        principalSchema: "public",
                        principalTable: "technologies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "work_experiences",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    employee_profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    tools = table.Column<string>(type: "jsonb", nullable: false),
                    third_parties = table.Column<List<string>>(type: "text[]", nullable: false),
                    frameworks = table.Column<List<string>>(type: "text[]", nullable: false),
                    version_control = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    project_management = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    responsibilities = table.Column<string>(type: "jsonb", nullable: false),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_work_experiences", x => x.id);
                    table.ForeignKey(
                        name: "fk_work_experiences_employee_profiles_employee_profile_id",
                        column: x => x.employee_profile_id,
                        principalSchema: "public",
                        principalTable: "employee_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_employee_languages_employee_profile_id",
                schema: "public",
                table: "employee_languages",
                column: "employee_profile_id");

            migrationBuilder.CreateIndex(
                name: "ix_personal_interests_employee_profile_id",
                schema: "public",
                table: "personal_interests",
                column: "employee_profile_id");

            migrationBuilder.CreateIndex(
                name: "ix_team_members_employee_profile_id",
                schema: "public",
                table: "team_members",
                column: "employee_profile_id");

            migrationBuilder.CreateIndex(
                name: "ix_team_required_technologies_technology_id",
                schema: "public",
                table: "team_required_technologies",
                column: "technology_id");

            migrationBuilder.CreateIndex(
                name: "ix_work_experiences_employee_profile_id",
                schema: "public",
                table: "work_experiences",
                column: "employee_profile_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "employee_languages",
                schema: "public");

            migrationBuilder.DropTable(
                name: "personal_interests",
                schema: "public");

            migrationBuilder.DropTable(
                name: "team_members",
                schema: "public");

            migrationBuilder.DropTable(
                name: "team_required_technologies",
                schema: "public");

            migrationBuilder.DropTable(
                name: "work_experiences",
                schema: "public");

            migrationBuilder.DropColumn(
                name: "description",
                schema: "public",
                table: "technology_categories");

            migrationBuilder.DropColumn(
                name: "description",
                schema: "public",
                table: "technologies");

            migrationBuilder.DropColumn(
                name: "description",
                schema: "public",
                table: "teams");

            migrationBuilder.DropColumn(
                name: "interests_weight",
                schema: "public",
                table: "teams");

            migrationBuilder.DropColumn(
                name: "members_json",
                schema: "public",
                table: "teams");

            migrationBuilder.DropColumn(
                name: "minimum_sfia_level",
                schema: "public",
                table: "teams");

            migrationBuilder.DropColumn(
                name: "psychological_weight",
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

            migrationBuilder.DropColumn(
                name: "technical_weight",
                schema: "public",
                table: "teams");

            migrationBuilder.DropColumn(
                name: "weight_criteria",
                schema: "public",
                table: "teams");

            migrationBuilder.DropColumn(
                name: "experience_level",
                schema: "public",
                table: "employee_technologies");

            migrationBuilder.DropColumn(
                name: "version",
                schema: "public",
                table: "employee_technologies");

            migrationBuilder.AlterColumn<string>(
                name: "ai_analysis",
                schema: "public",
                table: "teams",
                type: "jsonb",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "members",
                schema: "public",
                table: "teams",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "required_technologies",
                schema: "public",
                table: "teams",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "timezone",
                schema: "public",
                table: "employee_profiles",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "specialization",
                schema: "public",
                table: "employee_profiles",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "sfia_level_general",
                schema: "public",
                table: "employee_profiles",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(3,1)",
                oldPrecision: 3,
                oldScale: 1);

            migrationBuilder.AlterColumn<string>(
                name: "mbti",
                schema: "public",
                table: "employee_profiles",
                type: "character varying(4)",
                maxLength: 4,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(4)",
                oldMaxLength: 4);

            migrationBuilder.AlterColumn<string>(
                name: "country",
                schema: "public",
                table: "employee_profiles",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "personal_interests",
                schema: "public",
                table: "employee_profiles",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "work_experience",
                schema: "public",
                table: "employee_profiles",
                type: "jsonb",
                nullable: false,
                defaultValue: "");
        }
    }
}
