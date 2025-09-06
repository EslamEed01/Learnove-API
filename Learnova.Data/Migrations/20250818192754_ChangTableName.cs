using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learnova.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_lessonVideos_Courses_CourseId",
                table: "lessonVideos");

            migrationBuilder.DropForeignKey(
                name: "FK_lessonVideos_Lessons_LessonId",
                table: "lessonVideos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_lessonVideos",
                table: "lessonVideos");

            migrationBuilder.RenameTable(
                name: "lessonVideos",
                newName: "LessonVideos");

            migrationBuilder.RenameIndex(
                name: "IX_lessonVideos_LessonId",
                table: "LessonVideos",
                newName: "IX_LessonVideos_LessonId");

            migrationBuilder.RenameIndex(
                name: "IX_lessonVideos_CourseId",
                table: "LessonVideos",
                newName: "IX_LessonVideos_CourseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LessonVideos",
                table: "LessonVideos",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LessonVideos_Courses_CourseId",
                table: "LessonVideos",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_LessonVideos_Lessons_LessonId",
                table: "LessonVideos",
                column: "LessonId",
                principalTable: "Lessons",
                principalColumn: "LessonId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LessonVideos_Courses_CourseId",
                table: "LessonVideos");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonVideos_Lessons_LessonId",
                table: "LessonVideos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LessonVideos",
                table: "LessonVideos");

            migrationBuilder.RenameTable(
                name: "LessonVideos",
                newName: "lessonVideos");

            migrationBuilder.RenameIndex(
                name: "IX_LessonVideos_LessonId",
                table: "lessonVideos",
                newName: "IX_lessonVideos_LessonId");

            migrationBuilder.RenameIndex(
                name: "IX_LessonVideos_CourseId",
                table: "lessonVideos",
                newName: "IX_lessonVideos_CourseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_lessonVideos",
                table: "lessonVideos",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_lessonVideos_Courses_CourseId",
                table: "lessonVideos",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_lessonVideos_Lessons_LessonId",
                table: "lessonVideos",
                column: "LessonId",
                principalTable: "Lessons",
                principalColumn: "LessonId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
