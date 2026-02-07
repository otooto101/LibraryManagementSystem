using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class BorrowRecordConfiguration : IEntityTypeConfiguration<BorrowRecord>
    {
        public void Configure(EntityTypeBuilder<BorrowRecord> builder)
        {
            // Enum mapping: Store as String in DB for readability, or Int for performance so i decided readability.
            builder.Property(br => br.Status)
                   .HasConversion<string>()
                   .HasMaxLength(20);

            builder.HasOne(br => br.Book)
                   .WithMany(b => b.BorrowRecords)
                   .HasForeignKey(br => br.BookId);

            builder.HasOne(br => br.Patron)
                   .WithMany(p => p.BorrowRecords)
                   .HasForeignKey(br => br.PatronId);

            builder.Property<byte[]>("RowVersion")
               .IsRowVersion();

        }
    }
}
