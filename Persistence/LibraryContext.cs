using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;
using System.Reflection;

namespace Persistence
{
    public class LibraryContext : IdentityDbContext<ApplicationUser>
    {
        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options)
        {
        }

        public DbSet<Book> Books => Set<Book>();
        public DbSet<Author> Authors => Set<Author>();
        public DbSet<Patron> Patrons => Set<Patron>();
        public DbSet<BorrowRecord> BorrowRecords => Set<BorrowRecord>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
