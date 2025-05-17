using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RolesAndAreasRefinement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "technical_areas",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_technical_areas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "specialized_roles",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    technical_area_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_specialized_roles", x => x.id);
                    table.ForeignKey(
                        name: "fk_specialized_roles_technical_areas_technical_area_id",
                        column: x => x.technical_area_id,
                        principalSchema: "public",
                        principalTable: "technical_areas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "employee_specialized_roles",
                schema: "public",
                columns: table => new
                {
                    employee_profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    specialized_role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    level = table.Column<int>(type: "integer", nullable: false),
                    years_experience = table.Column<int>(type: "integer", nullable: false),
                    id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_employee_specialized_roles", x => new { x.employee_profile_id, x.specialized_role_id });
                    table.ForeignKey(
                        name: "fk_employee_specialized_roles_employee_profiles_employee_profi",
                        column: x => x.employee_profile_id,
                        principalSchema: "public",
                        principalTable: "employee_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_employee_specialized_roles_specialized_roles_specialized_ro",
                        column: x => x.specialized_role_id,
                        principalSchema: "public",
                        principalTable: "specialized_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "specialized_role_skills",
                schema: "public",
                columns: table => new
                {
                    specialized_role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    technology_id = table.Column<Guid>(type: "uuid", nullable: false),
                    minimum_level = table.Column<int>(type: "integer", nullable: false),
                    id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_specialized_role_skills", x => new { x.specialized_role_id, x.technology_id });
                    table.ForeignKey(
                        name: "fk_specialized_role_skills_specialized_roles_specialized_role_",
                        column: x => x.specialized_role_id,
                        principalSchema: "public",
                        principalTable: "specialized_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_specialized_role_skills_technologies_technology_id",
                        column: x => x.technology_id,
                        principalSchema: "public",
                        principalTable: "technologies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_employee_specialized_roles_specialized_role_id",
                schema: "public",
                table: "employee_specialized_roles",
                column: "specialized_role_id");

            migrationBuilder.CreateIndex(
                name: "ix_specialized_role_skills_technology_id",
                schema: "public",
                table: "specialized_role_skills",
                column: "technology_id");

            migrationBuilder.CreateIndex(
                name: "ix_specialized_roles_technical_area_id_name",
                schema: "public",
                table: "specialized_roles",
                columns: new[] { "technical_area_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_technical_areas_name",
                schema: "public",
                table: "technical_areas",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "employee_specialized_roles",
                schema: "public");

            migrationBuilder.DropTable(
                name: "specialized_role_skills",
                schema: "public");

            migrationBuilder.DropTable(
                name: "specialized_roles",
                schema: "public");

            migrationBuilder.DropTable(
                name: "technical_areas",
                schema: "public");
        }
    }
}
