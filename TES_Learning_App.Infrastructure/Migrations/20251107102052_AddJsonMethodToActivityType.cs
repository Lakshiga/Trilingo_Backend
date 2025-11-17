using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TES_Learning_App.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddJsonMethodToActivityType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JsonMethod",
                table: "ActivityTypes",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JsonMethod",
                table: "ActivityTypes");
        }
    }
}
