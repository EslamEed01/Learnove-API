using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Learnova.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAppRoleCol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1ae940de-7f08-4fd5-a228-2236ec81a49b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "79dc0978-7539-4a55-9809-38116e9193ac");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8f01ecf3-877b-46cc-a32b-10c72b78ae25");

            migrationBuilder.CreateTable(
                name: "ApplicationRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDefault = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NormalizedName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationRoles", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "03d6368f-32c2-4d1f-867c-2f4d8d8efd3e", "b19b9133-6c62-46d0-adb2-8bcd15b27b20", "Admin", "ADMIN" },
                    { "bbca7f54-82c1-4d3c-b021-55a1484212d4", "6441ba17-cdc7-4fbe-8ad6-b94db3f61f1e", "Student", "STUDENT" },
                    { "fc14d3a4-74d5-4865-969d-cad4b243edf5", "cbfb63c3-d28d-4daa-bc8d-02d46fbafa35", "instructor", "instructor" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationRoles");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "03d6368f-32c2-4d1f-867c-2f4d8d8efd3e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bbca7f54-82c1-4d3c-b021-55a1484212d4");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "fc14d3a4-74d5-4865-969d-cad4b243edf5");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1ae940de-7f08-4fd5-a228-2236ec81a49b", "bbcc8f47-352b-4746-aa3e-ab4e513cb8b1", "Admin", "ADMIN" },
                    { "79dc0978-7539-4a55-9809-38116e9193ac", "977b6448-0a95-48c7-867c-3d8c5aa99b15", "Student", "STUDENT" },
                    { "8f01ecf3-877b-46cc-a32b-10c72b78ae25", "88c85a55-09c4-4aba-98de-d252036197eb", "instructor", "instructor" }
                });
        }
    }
}
