using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexOnAuthor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Authors_FullName",
                table: "Authors");

            migrationBuilder.CreateIndex(
                name: "IX_Authors_FirstName_LastName_DateOfBirth",
                table: "Authors",
                columns: new[] { "FirstName", "LastName", "DateOfBirth" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Authors_FirstName_LastName_DateOfBirth",
                table: "Authors");

            migrationBuilder.CreateIndex(
                name: "IX_Authors_FullName",
                table: "Authors",
                column: "FullName");
        }
    }
}
