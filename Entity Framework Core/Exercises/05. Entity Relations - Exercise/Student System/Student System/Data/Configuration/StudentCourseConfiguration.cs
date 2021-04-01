namespace P01_StudentSystem.Data.Configuration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Models;

    public class StudentCourseConfiguration : IEntityTypeConfiguration<StudentCourse>
    {
        public void Configure(EntityTypeBuilder<StudentCourse> entity)
        {
            entity
                .HasKey(sc => new { sc.StudentId, sc.CourseId });

            entity
                .HasOne(s => s.Student)
                .WithMany(sc => sc.CourseEnrollments)
                .HasForeignKey(s => s.StudentId);

            entity
                .HasOne(c => c.Course)
                .WithMany(sc => sc.StudentsEnrolled)
                .HasForeignKey(c => c.CourseId);
        }
    }
}
