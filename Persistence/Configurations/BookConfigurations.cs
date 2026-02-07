using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(b => b.ISBN)
                .IsRequired()
                .HasMaxLength(17);

            builder.HasOne(b => b.Author)
               .WithMany(a => a.Books)
               .HasForeignKey(b => b.AuthorId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(b => b.ISBN)
                .IsUnique();
        }
    }
}
