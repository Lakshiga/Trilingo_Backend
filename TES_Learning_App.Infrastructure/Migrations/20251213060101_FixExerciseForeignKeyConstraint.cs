using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TES_Learning_App.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixExerciseForeignKeyConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentProgresses_Exercises_ExerciseId",
                table: "StudentProgresses");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentProgresses_Exercises_ExerciseId",
                table: "StudentProgresses",
                column: "ExerciseId",
                principalTable: "Exercises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentProgresses_Exercises_ExerciseId",
                table: "StudentProgresses");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentProgresses_Exercises_ExerciseId",
                table: "StudentProgresses",
                column: "ExerciseId",
                principalTable: "Exercises",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
