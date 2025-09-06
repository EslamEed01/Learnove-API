using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Learnova.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "036ce958-ad7d-4604-8d23-ac49ccf9e247");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4e94ed8d-0b2c-4b77-846d-5951e8cfbce8");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c0ccda1d-07ec-48af-90b5-e862d74d41d9");

            migrationBuilder.CreateTable(
                name: "Review",
                columns: table => new
                {
                    ReviewId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReviewDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Review", x => x.ReviewId);
                    table.ForeignKey(
                        name: "FK_Review_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Review_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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
                name: "IX_Review_CourseId",
                table: "Review",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Review_UserId",
                table: "Review",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Review");

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

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "036ce958-ad7d-4604-8d23-ac49ccf9e247", "7c3ec419-6473-41c3-bc99-bcba8b27ab77", "Student", "STUDENT" },
                    { "4e94ed8d-0b2c-4b77-846d-5951e8cfbce8", "1318f5f6-a59c-4a7b-ad7c-0aa153fbddad", "Admin", "ADMIN" },
                    { "c0ccda1d-07ec-48af-90b5-e862d74d41d9", "081b0584-83e5-4e46-b975-13b8ec959906", "instructor", "instructor" }
                });
        }
    }
}
