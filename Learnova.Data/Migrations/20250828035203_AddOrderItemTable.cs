using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Learnova.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderItemTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    OrderItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(65,30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.OrderItemId);
                    table.ForeignKey(
                        name: "FK_OrderItems_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "2bfb6fc7-ab8d-420d-939c-b879a601eab5", "dda28cea-706f-4817-9b5a-199ae92d21b9", "instructor", "instructor" },
                    { "56d458ed-63c7-4386-bc14-eec4be283c53", "93666dd5-1277-48c7-9ad8-ca9016403127", "Admin", "ADMIN" },
                    { "c8da358e-6fc4-4baa-9323-14dfb1bd6a13", "0355a4f4-1d2e-4c1a-ac0b-602ff835aad3", "Student", "STUDENT" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_CourseId",
                table: "OrderItems",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2bfb6fc7-ab8d-420d-939c-b879a601eab5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "56d458ed-63c7-4386-bc14-eec4be283c53");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c8da358e-6fc4-4baa-9323-14dfb1bd6a13");

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
    }
}
