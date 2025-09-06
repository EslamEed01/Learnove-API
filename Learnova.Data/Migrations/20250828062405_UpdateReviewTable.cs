using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Learnova.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReviewTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Review_AspNetUsers_UserId",
                table: "Review");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_Courses_CourseId",
                table: "Review");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Review",
                table: "Review");

            migrationBuilder.DropIndex(
                name: "IX_Review_UserId",
                table: "Review");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "225da6e3-5edb-4bad-bcf0-969398c079af");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "43492771-1ffc-4af0-827c-1aff880a4784");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a35fa41c-8915-463b-badd-b3b78bfffe27");

            migrationBuilder.RenameTable(
                name: "Review",
                newName: "Reviews");

            migrationBuilder.RenameIndex(
                name: "IX_Review_CourseId",
                table: "Reviews",
                newName: "IX_Reviews_CourseId");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Reviews",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reviews",
                table: "Reviews",
                column: "ReviewId");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4823bc90-192b-4de6-a37b-dc64cea887d8", "e68fa151-876e-4173-b703-2580092c7f76", "instructor", "instructor" },
                    { "5941ef81-7dad-441d-9488-d2c14598d0e0", "f47b2862-93aa-482a-8eb0-73b44c6658ee", "Student", "STUDENT" },
                    { "6f06a626-302f-42a8-aa04-71a12dc1d82b", "32b2a5cd-57c6-4444-a395-e94b86ec3f85", "Admin", "ADMIN" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId_CourseId",
                table: "Reviews",
                columns: new[] { "UserId", "CourseId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_AspNetUsers_UserId",
                table: "Reviews",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Courses_CourseId",
                table: "Reviews",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_AspNetUsers_UserId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Courses_CourseId",
                table: "Reviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reviews",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_UserId_CourseId",
                table: "Reviews");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4823bc90-192b-4de6-a37b-dc64cea887d8");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5941ef81-7dad-441d-9488-d2c14598d0e0");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6f06a626-302f-42a8-aa04-71a12dc1d82b");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Reviews");

            migrationBuilder.RenameTable(
                name: "Reviews",
                newName: "Review");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_CourseId",
                table: "Review",
                newName: "IX_Review_CourseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Review",
                table: "Review",
                column: "ReviewId");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "225da6e3-5edb-4bad-bcf0-969398c079af", "8f62bf84-f41f-4309-8167-be588666b992", "instructor", "instructor" },
                    { "43492771-1ffc-4af0-827c-1aff880a4784", "2a3229b8-6aa0-492d-ba10-b4a08363e2a1", "Student", "STUDENT" },
                    { "a35fa41c-8915-463b-badd-b3b78bfffe27", "62cfbd9a-29d9-4add-b3ba-b83122f702f9", "Admin", "ADMIN" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Review_UserId",
                table: "Review",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Review_AspNetUsers_UserId",
                table: "Review",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Courses_CourseId",
                table: "Review",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
