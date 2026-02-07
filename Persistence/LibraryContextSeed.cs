using Domain.Constants;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;

namespace Persistence
{
    public class LibraryContextSeed
    {
        public static async Task SeedAsync(
            LibraryContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            // Ensure Migrations are applied
            if (context.Database.IsSqlServer())
            {
                await context.Database.MigrateAsync();
            }

            // 1. Seed Identity (Roles & Users)
            await SeedIdentityAsync(userManager, roleManager);

            // 2. Seed Authors
            if (!await context.Authors.AnyAsync())
            {
                await context.Authors.AddRangeAsync(GetPreconfiguredAuthors());
                await context.SaveChangesAsync();
            }

            // 3. Seed Books
            if (!await context.Books.AnyAsync())
            {
                var authors = await context.Authors.ToListAsync();
                await context.Books.AddRangeAsync(GetPreconfiguredBooks(authors));
                await context.SaveChangesAsync();
            }

            // 4. Seed Patrons
            if (!await context.Patrons.AnyAsync())
            {
                await context.Patrons.AddRangeAsync(GetPreconfiguredPatrons());
                await context.SaveChangesAsync();
            }

            // 5. Seed Borrow Records
            if (!await context.BorrowRecords.AnyAsync())
            {
                var books = await context.Books.ToListAsync();
                var patrons = await context.Patrons.ToListAsync();

                if (books.Any() && patrons.Any())
                {
                    await context.BorrowRecords.AddRangeAsync(GetPreconfiguredBorrowRecords(books, patrons));
                    await context.SaveChangesAsync();
                }
            }
        }

        private static async Task SeedIdentityAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Seed Roles
            if (!await roleManager.RoleExistsAsync(Roles.Admin))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.Admin));
                await roleManager.CreateAsync(new IdentityRole(Roles.Librarian));
            }

            // Seed Default Admin User
            var defaultAdmin = new ApplicationUser
            {
                UserName = "admin@library.com",
                Email = "admin@library.com",
                FirstName = "System",
                LastName = "Admin",
                EmailConfirmed = true
            };

            if (userManager.Users.All(u => u.UserName != defaultAdmin.UserName))
            {
                await userManager.CreateAsync(defaultAdmin, "Admin123!");
                await userManager.AddToRoleAsync(defaultAdmin, Roles.Admin);
            }
        }

        static List<Author> GetPreconfiguredAuthors()
        {
            return new List<Author>
            {
                new() { FirstName = "Jon", LastName = "P. Smith", DateOfBirth = new DateOnly(1962, 10, 20) },
                new() { FirstName = "Robert", LastName = "Martin", DateOfBirth = new DateOnly(1952, 12, 05) }
            };
        }

        static List<Book> GetPreconfiguredBooks(List<Author> authors)
        {
            var smith = authors.First(a => a.LastName.Contains("Smith"));
            var martin = authors.First(a => a.LastName.Contains("Martin"));

            return new List<Book>
            {
                new()
                {
                    Title = "EF Core in Action",
                    AuthorId = smith.AuthorId,
                    PublicationYear = 2021,
                    ISBN = "978-1617298370",
                    Quantity = 10,
                    Description = "The definitive guide to EF Core."
                },
                new()
                {
                    Title = "Clean Code",
                    AuthorId = martin.AuthorId,
                    PublicationYear = 2008,
                    ISBN = "978-0132350884",
                    Quantity = 5,
                    Description = "A handbook of agile software craftsmanship."
                }
            };
        }

        static List<Patron> GetPreconfiguredPatrons()
        {
            return new List<Patron>
            {
                new() { FirstName = "Hermione", LastName = "Granger", Email = "h.granger@hogwarts.edu" },
                new() { FirstName = "Harry", LastName = "Potter", Email = "h.potter@hogwarts.edu" }
            };
        }

        static List<BorrowRecord> GetPreconfiguredBorrowRecords(List<Book> books, List<Patron> patrons)
        {
            var book = books.First(b => b.Title == "EF Core in Action");
            var patron = patrons.First(p => p.LastName == "Granger");

            var activeRecord = new BorrowRecord(book.BookId, patron.PatronId, 14);

            var overdueRecord = new BorrowRecord(book.BookId, patron.PatronId, 7);
            overdueRecord.BorrowDate = DateTime.UtcNow.AddDays(-20);
            overdueRecord.DueDate = DateTime.UtcNow.AddDays(-13);
            // Status remains 'Borrowed' so the Background Job can find and update it

            var returnedRecord = new BorrowRecord(books.Last().BookId, patrons.Last().PatronId, 14);
            returnedRecord.MarkAsReturned();

            return new List<BorrowRecord> { activeRecord, overdueRecord, returnedRecord };
        }
    }
}