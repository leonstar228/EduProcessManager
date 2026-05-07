using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduProcessManager.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentGroupsAndTestPublishing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StudentGroupId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StudentGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TestStudentGroups",
                columns: table => new
                {
                    TestId = table.Column<int>(type: "int", nullable: false),
                    StudentGroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestStudentGroups", x => new { x.TestId, x.StudentGroupId });
                    table.ForeignKey(
                        name: "FK_TestStudentGroups_StudentGroups_StudentGroupId",
                        column: x => x.StudentGroupId,
                        principalTable: "StudentGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestStudentGroups_Tests_TestId",
                        column: x => x.TestId,
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_StudentGroupId",
                table: "AspNetUsers",
                column: "StudentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_TestStudentGroups_StudentGroupId",
                table: "TestStudentGroups",
                column: "StudentGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_StudentGroups_StudentGroupId",
                table: "AspNetUsers",
                column: "StudentGroupId",
                principalTable: "StudentGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_StudentGroups_StudentGroupId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "TestStudentGroups");

            migrationBuilder.DropTable(
                name: "StudentGroups");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_StudentGroupId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "StudentGroupId",
                table: "AspNetUsers");
        }
    }
}
