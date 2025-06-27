using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GDPRCompilance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "data_deletion_requests",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    request_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    scheduled_deletion_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    completed_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    data_types = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    cancellation_reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_data_deletion_requests", x => x.id);
                    table.ForeignKey(
                        name: "fk_data_deletion_requests_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "privacy_audit_logs",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    action = table.Column<int>(type: "integer", nullable: false),
                    details = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ip_address = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    user_agent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_privacy_audit_logs", x => x.id);
                    table.ForeignKey(
                        name: "fk_privacy_audit_logs_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_privacy_consents",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    profile_data_processing = table.Column<bool>(type: "boolean", nullable: false),
                    team_matching_analysis = table.Column<bool>(type: "boolean", nullable: false),
                    performance_tracking = table.Column<bool>(type: "boolean", nullable: false),
                    communication_preferences = table.Column<bool>(type: "boolean", nullable: false),
                    analytics_and_improvement = table.Column<bool>(type: "boolean", nullable: false),
                    last_updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_privacy_consents", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_privacy_consents_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_data_deletion_requests_user_id",
                schema: "public",
                table: "data_deletion_requests",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_privacy_audit_logs_timestamp",
                schema: "public",
                table: "privacy_audit_logs",
                column: "timestamp");

            migrationBuilder.CreateIndex(
                name: "ix_privacy_audit_logs_user_id",
                schema: "public",
                table: "privacy_audit_logs",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_privacy_consents_user_id",
                schema: "public",
                table: "user_privacy_consents",
                column: "user_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "data_deletion_requests",
                schema: "public");

            migrationBuilder.DropTable(
                name: "privacy_audit_logs",
                schema: "public");

            migrationBuilder.DropTable(
                name: "user_privacy_consents",
                schema: "public");
        }
    }
}
