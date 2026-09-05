using APITask.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace APITask.Data.Configurations
{
    public class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
    {
        public void Configure(EntityTypeBuilder<Assignment> builder)
        {
            builder.ToTable("Tasks");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Title)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(a => a.Description)
                .IsRequired(false)
                .HasMaxLength(500);

            builder.Property(a => a.IsCompleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(a => a.CompletedAt)
                .IsRequired(false);

            builder.Property(a => a.CreatedAt)
                .IsRequired();

            builder.HasIndex(a => a.Title);
        }
    }
}
