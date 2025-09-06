using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Learnova.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentIntentIdToPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<string>(
                name: "PaymentIntentId",
                table: "Payments",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "PaymentIntentId",
                table: "Payments");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "2bfb6fc7-ab8d-420d-939c-b879a601eab5", "dda28cea-706f-4817-9b5a-199ae92d21b9", "instructor", "instructor" },
                    { "56d458ed-63c7-4386-bc14-eec4be283c53", "93666dd5-1277-48c7-9ad8-ca9016403127", "Admin", "ADMIN" },
                    { "c8da358e-6fc4-4baa-9323-14dfb1bd6a13", "0355a4f4-1d2e-4c1a-ac0b-602ff835aad3", "Student", "STUDENT" }
                });
        }
    }
}
