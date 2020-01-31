namespace P01_StudentSystem.Data.Configuration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Models;

    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> entity)
        {
            entity
                .HasKey(c => c.CourseId);

            entity
                .Property(c => c.Name)
                .HasMaxLength(80)
                .IsUnicode(true)
                .IsRequired(true);

            entity
                .Property(c => c.Description)
                .IsUnicode(true)
                .IsRequired(false);

            entity
                .Property(c => c.StartDate)
                .HasColumnType("DATETIME2")
                .IsRequired(true);

            entity
                .Property(c => c.EndDate)
                .HasColumnType("DATETIME2")
                .IsRequired(true);

            entity
                .Property(c => c.Price)
                .IsRequired(true);

            entity
                .HasMany(sc => sc.StudentsEnrolled)
                .WithOne(c => c.Course)
                .HasForeignKey(sc => sc.CourseId);

            entity
                .HasMany(r => r.Resources)
                .WithOne(c => c.Course)
                .HasForeignKey(r => r.CourseId);

            entity
                .HasMany(h => h.HomeworkSubmissions)
                .WithOne(c => c.Course)
                .HasForeignKey(h => h.CourseId);
        }
    }
}
