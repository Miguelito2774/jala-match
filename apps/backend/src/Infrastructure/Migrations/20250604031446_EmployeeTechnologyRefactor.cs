using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EmployeeTechnologyRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "experience_level",
                schema: "public",
                table: "employee_technologies");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "experience_level",
                schema: "public",
                table: "employee_technologies",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
