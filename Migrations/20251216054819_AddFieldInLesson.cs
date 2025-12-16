using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JournalApi.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldInLesson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Topic",
                table: "Lessons",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Topic",
                table: "Lessons");
        }
    }
}
