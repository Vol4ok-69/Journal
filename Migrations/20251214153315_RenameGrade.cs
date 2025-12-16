using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JournalApi.Migrations
{
    /// <inheritdoc />
    public partial class RenameGrade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Grade",
                table: "Grades",
                newName: "Value");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Value",
                table: "Grades",
                newName: "Grade");
        }
    }
}
