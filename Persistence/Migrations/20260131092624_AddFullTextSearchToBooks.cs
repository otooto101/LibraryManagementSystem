using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFullTextSearchToBooks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create the Full-Text Catalog
            // We use suppressTransaction: true because SQL Server requires FTS catalog 
            // creation to happen outside of a multi-statement transaction in some environments
            migrationBuilder.Sql("CREATE FULLTEXT CATALOG BookCatalog AS DEFAULT;", suppressTransaction: true);
            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON Books(Title) KEY INDEX PK_Books ON BookCatalog;", suppressTransaction: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FULLTEXT INDEX ON Books;", suppressTransaction: true);
            migrationBuilder.Sql("DROP FULLTEXT CATALOG BookCatalog;", suppressTransaction: true);
        }
    }
}
