using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HobbyMood.Data.Migrations
{
    /// <inheritdoc />
    public partial class dtoupdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Experiences_Hobbies_HobbyId",
                table: "Experiences");

            migrationBuilder.AlterColumn<int>(
                name: "HobbyId",
                table: "Experiences",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Experiences_Hobbies_HobbyId",
                table: "Experiences",
                column: "HobbyId",
                principalTable: "Hobbies",
                principalColumn: "HobbyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Experiences_Hobbies_HobbyId",
                table: "Experiences");

            migrationBuilder.AlterColumn<int>(
                name: "HobbyId",
                table: "Experiences",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Experiences_Hobbies_HobbyId",
                table: "Experiences",
                column: "HobbyId",
                principalTable: "Hobbies",
                principalColumn: "HobbyId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
