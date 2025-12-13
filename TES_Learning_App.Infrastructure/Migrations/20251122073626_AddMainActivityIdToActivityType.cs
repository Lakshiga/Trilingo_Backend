using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TES_Learning_App.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMainActivityIdToActivityType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Add column as nullable first (to handle existing data)
            migrationBuilder.AddColumn<int>(
                name: "MainActivityId",
                table: "ActivityTypes",
                type: "int",
                nullable: true);

            // Step 2: Set default value for existing records
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM MainActivities)
                BEGIN
                    UPDATE ActivityTypes 
                    SET MainActivityId = (SELECT TOP 1 Id FROM MainActivities ORDER BY Id)
                    WHERE MainActivityId IS NULL
                END
            ");

            // Step 3: Make column NOT NULL after setting defaults
            migrationBuilder.AlterColumn<int>(
                name: "MainActivityId",
                table: "ActivityTypes",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActivityTypes_MainActivityId",
                table: "ActivityTypes",
                column: "MainActivityId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityTypes_MainActivities_MainActivityId",
                table: "ActivityTypes",
                column: "MainActivityId",
                principalTable: "MainActivities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityTypes_MainActivities_MainActivityId",
                table: "ActivityTypes");

            migrationBuilder.DropIndex(
                name: "IX_ActivityTypes_MainActivityId",
                table: "ActivityTypes");

            migrationBuilder.DropColumn(
                name: "MainActivityId",
                table: "ActivityTypes");
        }
    }
}
