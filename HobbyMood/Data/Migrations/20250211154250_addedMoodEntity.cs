using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HobbyMood.Data.Migrations
{
    /// <inheritdoc />
    public partial class addedMoodEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExperienceDuration",
                table: "Experiences",
                newName: "DurationinHours");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DurationinHours",
                table: "Experiences",
                newName: "ExperienceDuration");
        }
    }
}
