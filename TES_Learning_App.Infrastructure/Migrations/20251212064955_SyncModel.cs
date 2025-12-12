using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TES_Learning_App.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_StudentProgresses_StudentId",
                table: "StudentProgresses",
                newName: "IX_StudentProgress_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentProgresses_ActivityId",
                table: "StudentProgresses",
                newName: "IX_StudentProgress_ActivityId");

            migrationBuilder.AddColumn<bool>(
                name: "IsFirstLogin",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MustChangePassword",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PasswordResetOtp",
                table: "Users",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PasswordResetOtpAttempts",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "PasswordResetOtpExpiry",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordResetToken",
                table: "Users",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PasswordResetTokenExpiry",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AttemptNumber",
                table: "StudentProgresses",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "StudentProgresses",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxScore",
                table: "StudentProgresses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "StudentProgresses",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TimeSpentSeconds",
                table: "StudentProgresses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_StudentProgress_CompletedAt",
                table: "StudentProgresses",
                column: "CompletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StudentProgress_StudentId_ActivityId",
                table: "StudentProgresses",
                columns: new[] { "StudentId", "ActivityId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StudentProgress_CompletedAt",
                table: "StudentProgresses");

            migrationBuilder.DropIndex(
                name: "IX_StudentProgress_StudentId_ActivityId",
                table: "StudentProgresses");

            migrationBuilder.DropColumn(
                name: "IsFirstLogin",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MustChangePassword",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PasswordResetOtp",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PasswordResetOtpAttempts",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PasswordResetOtpExpiry",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PasswordResetToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PasswordResetTokenExpiry",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AttemptNumber",
                table: "StudentProgresses");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "StudentProgresses");

            migrationBuilder.DropColumn(
                name: "MaxScore",
                table: "StudentProgresses");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "StudentProgresses");

            migrationBuilder.DropColumn(
                name: "TimeSpentSeconds",
                table: "StudentProgresses");

            migrationBuilder.RenameIndex(
                name: "IX_StudentProgress_StudentId",
                table: "StudentProgresses",
                newName: "IX_StudentProgresses_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentProgress_ActivityId",
                table: "StudentProgresses",
                newName: "IX_StudentProgresses_ActivityId");
        }
    }
}
