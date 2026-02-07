using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class AuthorConfiguration : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> builder)
        {
            builder.Property(a => a.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.FullName)
                .HasComputedColumnSql(
                    "[FirstName] + ' ' + [LastName]",
                    stored: true)
                .HasMaxLength(450);

            builder.Property(a => a.DateOfBirth).HasColumnType("date");

            builder.HasIndex(a => new { a.FirstName, a.LastName, a.DateOfBirth })
                .IsUnique();

            builder.HasIndex(a => a.DateOfBirth);
        }
    }
}
