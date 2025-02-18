using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HobbyMood.Data.Migrations
{
    /// <inheritdoc />
    public partial class addedExperienceMoodEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExperienceMoods",
                columns: table => new
                {
                    ExperienceMoodId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExperienceId = table.Column<int>(type: "int", nullable: false),
                    MoodId = table.Column<int>(type: "int", nullable: false),
                    MoodIntensityBefore = table.Column<int>(type: "int", nullable: false),
                    MoodIntensityAfter = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExperienceMoods", x => x.ExperienceMoodId);
                    table.ForeignKey(
                        name: "FK_ExperienceMoods_Experiences_ExperienceId",
                        column: x => x.ExperienceId,
                        principalTable: "Experiences",
                        principalColumn: "ExperienceId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExperienceMoods_Moods_MoodId",
                        column: x => x.MoodId,
                        principalTable: "Moods",
                        principalColumn: "MoodId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExperienceMoods_ExperienceId",
                table: "ExperienceMoods",
                column: "ExperienceId");

            migrationBuilder.CreateIndex(
                name: "IX_ExperienceMoods_MoodId",
                table: "ExperienceMoods",
                column: "MoodId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExperienceMoods");
        }
    }
}
