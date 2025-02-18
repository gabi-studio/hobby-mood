using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HobbyMood.Data.Migrations
{
    /// <inheritdoc />
    public partial class hobbycategory2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "Hobbies",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Hobbies");
        }
    }
}
