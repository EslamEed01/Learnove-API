using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Learnova.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokenTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<bool>(
                name: "IsDisabled",
                table: "AspNetUsers",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                columns: table => new
                {
                    AppUserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Token = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExpiresOn = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    RevokedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => new { x.AppUserId, x.Id });
                    table.ForeignKey(
                        name: "FK_RefreshToken_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshToken");

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

            migrationBuilder.DropColumn(
                name: "IsDisabled",
                table: "AspNetUsers");

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
    }
}
