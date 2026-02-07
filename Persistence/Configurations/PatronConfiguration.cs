using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class PatronConfiguration : IEntityTypeConfiguration<Patron>
    {
        public void Configure(EntityTypeBuilder<Patron> builder)
        {
            builder.Property(p => p.FirstName).IsRequired().HasMaxLength(50);
            builder.Property(p => p.LastName).IsRequired().HasMaxLength(50);
            builder.Property(p => p.Email).IsRequired().HasMaxLength(100);

            builder.HasIndex(p => new { p.LastName, p.FirstName });
            builder.HasIndex(p => p.FirstName);
            builder.HasIndex(p => p.Email).IsUnique();
        }
    }
}
