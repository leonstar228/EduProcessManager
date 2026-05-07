using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduProcessManager.Migrations
{
    /// <inheritdoc />
    public partial class AddTestScoreScale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ScoreScale",
                table: "Tests",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScoreScale",
                table: "Tests");
        }
    }
}
