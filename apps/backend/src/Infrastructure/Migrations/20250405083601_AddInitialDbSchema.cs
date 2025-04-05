using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInitialDbSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_active",
                schema: "public",
                table: "users");

            migrationBuilder.AlterColumn<string>(
                name: "profile_picture_url",
                schema: "public",
                table: "users",
                type: "character varying(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "password_hash",
                schema: "public",
                table: "users",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "employee_profiles",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    availability = table.Column<bool>(type: "boolean", nullable: false),
                    country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    timezone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    sfia_level_general = table.Column<int>(type: "integer", nullable: true),
                    specialization = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    mbti = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: true),
                    work_experience = table.Column<string>(type: "jsonb", nullable: false),
                    personal_interests = table.Column<string>(type: "jsonb", nullable: false),
                    verification_status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    verification_notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_employee_profiles", x => x.id);
                    table.ForeignKey(
                        name: "fk_employee_profiles_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "teams",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: false),
                    required_technologies = table.Column<string>(type: "jsonb", nullable: false),
                    members = table.Column<string>(type: "jsonb", nullable: false),
                    ai_analysis = table.Column<string>(type: "jsonb", nullable: false),
                    compatibility_score = table.Column<int>(type: "integer", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_teams", x => x.id);
                    table.ForeignKey(
                        name: "fk_teams_users_creator_id",
                        column: x => x.creator_id,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "technology_categories",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_technology_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "profile_verifications",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    employee_profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reviewer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sfia_proposed = table.Column<int>(type: "integer", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    notes = table.Column<string>(type: "text", nullable: true),
                    reviewed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_profile_verifications", x => x.id);
                    table.ForeignKey(
                        name: "fk_profile_verifications_employee_profiles_employee_profile_id",
                        column: x => x.employee_profile_id,
                        principalSchema: "public",
                        principalTable: "employee_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_profile_verifications_users_reviewer_id",
                        column: x => x.reviewer_id,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "technologies",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    version = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_technologies", x => x.id);
                    table.ForeignKey(
                        name: "fk_technologies_technology_categories_category_id",
                        column: x => x.category_id,
                        principalSchema: "public",
                        principalTable: "technology_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "employee_technologies",
                schema: "public",
                columns: table => new
                {
                    employee_profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    technology_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sfia_level = table.Column<int>(type: "integer", nullable: false),
                    years_experience = table.Column<decimal>(type: "numeric(3,1)", nullable: false),
                    id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_employee_technologies", x => new { x.employee_profile_id, x.technology_id });
                    table.ForeignKey(
                        name: "fk_employee_technologies_employee_profiles_employee_profile_id",
                        column: x => x.employee_profile_id,
                        principalSchema: "public",
                        principalTable: "employee_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_employee_technologies_technologies_technology_id",
                        column: x => x.technology_id,
                        principalSchema: "public",
                        principalTable: "technologies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_employee_profiles_user_id",
                schema: "public",
                table: "employee_profiles",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_employee_technologies_technology_id",
                schema: "public",
                table: "employee_technologies",
                column: "technology_id");

            migrationBuilder.CreateIndex(
                name: "ix_profile_verifications_employee_profile_id",
                schema: "public",
                table: "profile_verifications",
                column: "employee_profile_id");

            migrationBuilder.CreateIndex(
                name: "ix_profile_verifications_reviewer_id",
                schema: "public",
                table: "profile_verifications",
                column: "reviewer_id");

            migrationBuilder.CreateIndex(
                name: "ix_teams_creator_id",
                schema: "public",
                table: "teams",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "ix_technologies_category_id",
                schema: "public",
                table: "technologies",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_technology_categories_name",
                schema: "public",
                table: "technology_categories",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "employee_technologies",
                schema: "public");

            migrationBuilder.DropTable(
                name: "profile_verifications",
                schema: "public");

            migrationBuilder.DropTable(
                name: "teams",
                schema: "public");

            migrationBuilder.DropTable(
                name: "technologies",
                schema: "public");

            migrationBuilder.DropTable(
                name: "employee_profiles",
                schema: "public");

            migrationBuilder.DropTable(
                name: "technology_categories",
                schema: "public");

            migrationBuilder.AlterColumn<string>(
                name: "profile_picture_url",
                schema: "public",
                table: "users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(512)",
                oldMaxLength: 512);

            migrationBuilder.AlterColumn<string>(
                name: "password_hash",
                schema: "public",
                table: "users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                schema: "public",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
