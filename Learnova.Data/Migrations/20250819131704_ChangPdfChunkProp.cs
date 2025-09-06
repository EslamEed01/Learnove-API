using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learnova.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangPdfChunkProp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PdfChunk_pdfContents_PdfContentId",
                table: "PdfChunk");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PdfChunk",
                table: "PdfChunk");

            migrationBuilder.RenameTable(
                name: "PdfChunk",
                newName: "pdfChunks");

            migrationBuilder.RenameIndex(
                name: "IX_PdfChunk_PdfContentId",
                table: "pdfChunks",
                newName: "IX_pdfChunks_PdfContentId");

            migrationBuilder.AlterColumn<string>(
                name: "PineconeVectorId",
                table: "pdfChunks",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_pdfChunks",
                table: "pdfChunks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_pdfChunks_pdfContents_PdfContentId",
                table: "pdfChunks",
                column: "PdfContentId",
                principalTable: "pdfContents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_pdfChunks_pdfContents_PdfContentId",
                table: "pdfChunks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_pdfChunks",
                table: "pdfChunks");

            migrationBuilder.RenameTable(
                name: "pdfChunks",
                newName: "PdfChunk");

            migrationBuilder.RenameIndex(
                name: "IX_pdfChunks_PdfContentId",
                table: "PdfChunk",
                newName: "IX_PdfChunk_PdfContentId");

            migrationBuilder.UpdateData(
                table: "PdfChunk",
                keyColumn: "PineconeVectorId",
                keyValue: null,
                column: "PineconeVectorId",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "PineconeVectorId",
                table: "PdfChunk",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PdfChunk",
                table: "PdfChunk",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PdfChunk_pdfContents_PdfContentId",
                table: "PdfChunk",
                column: "PdfContentId",
                principalTable: "pdfContents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
