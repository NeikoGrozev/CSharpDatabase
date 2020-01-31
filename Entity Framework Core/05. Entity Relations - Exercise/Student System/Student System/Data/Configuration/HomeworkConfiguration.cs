namespace P01_StudentSystem.Data.Configuration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Models;

    public class HomeworkConfiguration : IEntityTypeConfiguration<Homework>
    {
        public void Configure(EntityTypeBuilder<Homework> entity)
        {
            entity
                .HasKey(h => h.HomeworkId);

            entity
                .Property(h => h.Content)
                .IsUnicode(false)
                .IsRequired(true);

            entity
                .Property(h => h.ContentType)
                .IsRequired(true);

            entity
                .Property(h => h.SubmissionTime)
                .HasColumnType("DATETIME2")
                .IsRequired(true);

            entity
                .Property(h => h.StudentId)
                .IsRequired(true);

            entity
                .HasOne(s => s.Student)
                .WithMany(h => h.HomeworkSubmissions)
                .HasForeignKey(s => s.StudentId);

            entity
                .Property(h => h.CourseId)
                .IsRequired(true);

            entity
                .HasOne(c => c.Course)
                .WithMany(h => h.HomeworkSubmissions)
                .HasForeignKey(c => c.CourseId);                
        }
    }
}
