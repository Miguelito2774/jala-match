using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PrivacyUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "analytics_and_improvement",
                schema: "public",
                table: "user_privacy_consents");

            migrationBuilder.DropColumn(
                name: "communication_preferences",
                schema: "public",
                table: "user_privacy_consents");

            migrationBuilder.DropColumn(
                name: "performance_tracking",
                schema: "public",
                table: "user_privacy_consents");

            migrationBuilder.DropColumn(
                name: "profile_data_processing",
                schema: "public",
                table: "user_privacy_consents");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "analytics_and_improvement",
                schema: "public",
                table: "user_privacy_consents",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "communication_preferences",
                schema: "public",
                table: "user_privacy_consents",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "performance_tracking",
                schema: "public",
                table: "user_privacy_consents",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "profile_data_processing",
                schema: "public",
                table: "user_privacy_consents",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
