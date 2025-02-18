using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HobbyMood.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateExperience : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Experiences_HobbyId",
                table: "Experiences",
                column: "HobbyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Experiences_Hobbies_HobbyId",
                table: "Experiences",
                column: "HobbyId",
                principalTable: "Hobbies",
                principalColumn: "HobbyId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Experiences_Hobbies_HobbyId",
                table: "Experiences");

            migrationBuilder.DropIndex(
                name: "IX_Experiences_HobbyId",
                table: "Experiences");
        }
    }
}
