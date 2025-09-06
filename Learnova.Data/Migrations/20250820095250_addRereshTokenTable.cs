using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Learnova.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addRereshTokenTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "31d87a71-4840-412c-918a-8c834a0f6aee");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7cd7b8b8-6c6b-4f4d-bf80-67addc1a7262");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "fccd2565-55a9-4cf1-bb30-96ece658ebcc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "31d87a71-4840-412c-918a-8c834a0f6aee", "a83bdb20-ed8b-4b7d-a732-bebacb1517f0", "Admin", "ADMIN" },
                    { "7cd7b8b8-6c6b-4f4d-bf80-67addc1a7262", "908fd207-a31e-4731-b50d-8741d6580019", "Student", "STUDENT" },
                    { "fccd2565-55a9-4cf1-bb30-96ece658ebcc", "4ed30f3c-0333-4d4e-9eed-c0ac3353a5aa", "Teacher", "TEACHER" }
                });
        }
    }
}
