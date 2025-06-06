using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProfileVerificationChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_profile_verifications_users_reviewer_id",
                schema: "public",
                table: "profile_verifications");

            migrationBuilder.AlterColumn<Guid>(
                name: "reviewer_id",
                schema: "public",
                table: "profile_verifications",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<DateTime>(
                name: "reviewed_at",
                schema: "public",
                table: "profile_verifications",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<DateTime>(
                name: "requested_at",
                schema: "public",
                table: "profile_verifications",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "fk_profile_verifications_users_reviewer_id",
                schema: "public",
                table: "profile_verifications",
                column: "reviewer_id",
                principalSchema: "public",
                principalTable: "users",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_profile_verifications_users_reviewer_id",
                schema: "public",
                table: "profile_verifications");

            migrationBuilder.DropColumn(
                name: "requested_at",
                schema: "public",
                table: "profile_verifications");

            migrationBuilder.AlterColumn<Guid>(
                name: "reviewer_id",
                schema: "public",
                table: "profile_verifications",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "reviewed_at",
                schema: "public",
                table: "profile_verifications",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_profile_verifications_users_reviewer_id",
                schema: "public",
                table: "profile_verifications",
                column: "reviewer_id",
                principalSchema: "public",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
