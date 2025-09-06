using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Learnova.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addSeedRols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "8198fb0a-81ed-4780-886b-40528e31dbf6", "3bccf335-ae1e-4f8c-bff8-bc0c0355476a", "Student", "STUDENT" },
                    { "bd50aa71-3f10-4ec8-bb38-e326cf90f6c5", "e48bb281-0a11-44b9-be0f-5acd32d64304", "Admin", "ADMIN" },
                    { "eac48e3b-c25e-4719-afa1-344bb378cfa6", "a6f9c52c-0805-40fa-8ea7-685d216a30bd", "instructor", "instructor" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8198fb0a-81ed-4780-886b-40528e31dbf6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bd50aa71-3f10-4ec8-bb38-e326cf90f6c5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "eac48e3b-c25e-4719-afa1-344bb378cfa6");
        }
    }
}
