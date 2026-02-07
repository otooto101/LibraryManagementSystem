using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRowVersionToBorrowRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "BorrowRecords",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patrons_FirstName",
                table: "Patrons",
                column: "FirstName");

            migrationBuilder.CreateIndex(
                name: "IX_Patrons_LastName_FirstName",
                table: "Patrons",
                columns: new[] { "LastName", "FirstName" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Patrons_FirstName",
                table: "Patrons");

            migrationBuilder.DropIndex(
                name: "IX_Patrons_LastName_FirstName",
                table: "Patrons");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "BorrowRecords");
        }
    }
}
