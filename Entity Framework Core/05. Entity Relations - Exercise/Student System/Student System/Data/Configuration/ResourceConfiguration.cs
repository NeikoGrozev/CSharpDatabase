namespace P01_StudentSystem.Data.Configuration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Models;

    public class ResourceConfiguration : IEntityTypeConfiguration<Resource>
    {
        public void Configure(EntityTypeBuilder<Resource> entity)
        {
            entity
                 .HasKey(r => r.ResourceId);

            entity
                .Property(r => r.Name)
                .HasMaxLength(50)
                .IsUnicode(true)
                .IsRequired(true);

            entity
                .Property(r => r.Url)
                .HasMaxLength(200)
                .IsRequired(true)
                .IsUnicode(true);

            entity
                .Property(r => r.ResourceType)
                .IsRequired(true);

            entity
                .Property(r => r.CourseId)
                .IsRequired(true);

            entity
                .HasOne(c => c.Course)
                .WithMany(r => r.Resources)
                .HasForeignKey(c => c.CourseId);
        }
    }
}
