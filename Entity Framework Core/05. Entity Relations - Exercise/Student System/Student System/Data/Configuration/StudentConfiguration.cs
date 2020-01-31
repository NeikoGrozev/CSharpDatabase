namespace P01_StudentSystem.Data.Configuration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Models;

    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> entity)
        {

            entity
                .HasKey(s => s.StudentId);

            entity
                .Property(s => s.Name)
                .HasMaxLength(100)
                .IsRequired(true)
                .IsUnicode(true);

            entity
                .Property(s => s.PhoneNumber)
                .HasColumnType("CHAR(10)")
                .IsRequired(false)
                .IsUnicode(false);

            entity
                .Property(s => s.RegisteredOn)
                .HasColumnType("DATETIME2")
                .IsRequired(true)
                .IsUnicode(false);

            entity
                .Property(s => s.Birthday)
                .HasColumnType("DATETIME2")
                .IsRequired(false);

            entity
                .HasMany(sc => sc.CourseEnrollments)
                .WithOne(s => s.Student)
                .HasForeignKey(sc => sc.StudentId);

            entity
                .HasMany(h => h.HomeworkSubmissions)
                .WithOne(s => s.Student)
                .HasForeignKey(h => h.StudentId);                
        }
    }
}
