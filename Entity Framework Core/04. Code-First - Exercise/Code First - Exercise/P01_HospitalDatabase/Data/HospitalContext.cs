namespace P01_HospitalDatabase.Data
{
    using Microsoft.EntityFrameworkCore;
    using P01_HospitalDatabase.Data.Models;

    public class HospitalContext : DbContext
    {
        public HospitalContext()
        {

        }

        public HospitalContext(DbContextOptions options)
            : base(options)
        {

        }
        public DbSet<Diagnose> Diagnoses { get; set; }

        public DbSet<Medicament> Medicaments { get; set; }

        public DbSet<Patient> Patients { get; set; }

        public DbSet<PatientMedicament> PatientsMedicaments { get; set; }

        public DbSet<Visitation> Visitations { get; set; }

        public DbSet<Doctor> Doctors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=NEIKO\\SQLEXPRESS; " +
                    "Database=Hospital; Integrated Security=true");
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfiguringPatientEntity(modelBuilder);
            ConfiguringVisitationEntity(modelBuilder);
            ConfiguringDoctorEntity(modelBuilder);
            ConfiguringPatientsMedicamentsEntity(modelBuilder);
            ConfiguringMedicamentEntity(modelBuilder);
            ConfiguringDiagnoseEntity(modelBuilder);
        }

        private void ConfiguringDiagnoseEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Diagnose>(entity =>
            {

                entity.HasKey(d => d.DiagnoseId);

                entity
                    .Property(d => d.Name)
                    .HasMaxLength(50)
                    .IsUnicode(true)
                    .IsRequired(true);

                entity
                     .Property(d => d.Comments)
                     .HasMaxLength(250)
                     .IsUnicode(true);

                entity
                    .HasOne(p => p.Patient)
                    .WithMany(d => d.Diagnoses)
                    .HasForeignKey(p => p.PatientId);
            });

        }

        private void ConfiguringMedicamentEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Medicament>(entity =>
            {
                entity.HasKey(m => m.MedicamentId);

                entity
                    .Property(m => m.Name)
                    .HasMaxLength(50)
                    .IsUnicode(true)
                    .IsRequired(true);

                entity
                    .HasMany(m => m.Prescriptions)
                    .WithOne(m => m.Medicament)
                    .HasForeignKey(m => m.MedicamentId);
            });
        }

        private void ConfiguringPatientEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasKey(p => p.PatientId);

                entity
                    .Property(p => p.FirstName)
                    .HasMaxLength(50)
                    .IsUnicode(true)
                    .IsRequired(true);

                entity
                    .Property(p => p.LastName)
                    .HasMaxLength(50)
                    .IsUnicode(true)
                    .IsRequired(true);

                entity
                    .Property(p => p.Address)
                    .HasMaxLength(250)
                    .IsUnicode(true)
                    .IsRequired(true);

                entity
                    .Property(p => p.Email)
                    .HasMaxLength(80)
                    .IsUnicode(false)
                    .IsRequired(true);

                entity
                    .HasMany(p => p.Visitations)
                    .WithOne(P => P.Patient)
                    .HasForeignKey(p => p.VisitationId);

                entity
                    .HasMany(p => p.Diagnoses)
                    .WithOne(p => p.Patient)
                    .HasForeignKey(p => p.DiagnoseId);

                entity
                .HasMany(p => p.Prescriptions)
                .WithOne(p => p.Patient)
                .HasForeignKey(p => p.PatientId);
            });

        }

        private void ConfiguringPatientsMedicamentsEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PatientMedicament>(entity =>
            {
                entity.HasKey(pm => new { pm.PatientId, pm.MedicamentId });

                entity
                    .HasOne(pm => pm.Patient)
                    .WithMany(pm => pm.Prescriptions)
                    .HasPrincipalKey(pm => pm.PatientId);

                entity
                    .HasOne(pm => pm.Medicament)
                    .WithMany(pm => pm.Prescriptions)
                    .HasForeignKey(pm => pm.MedicamentId);
            });
        }

        private void ConfiguringVisitationEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Visitation>(entity =>
            {
                entity.HasKey(v => v.VisitationId);

                entity
                    .Property(v => v.Date)
                    .IsRequired(true);

                entity
                    .Property(v => v.Comments)
                    .HasMaxLength(250)
                    .IsUnicode(true);

                entity
                    .HasOne(v => v.Patient)
                    .WithMany(p => p.Visitations)
                    .HasForeignKey(v => v.PatientId);

                entity
                    .HasOne(d => d.Doctor)
                    .WithMany(v => v.Visitations)
                    .HasForeignKey(d => d.DoctorId);
            });
        }

        private void ConfiguringDoctorEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.HasKey(d => d.DoctorId);

                entity
                    .Property(d => d.Name)
                    .HasMaxLength(100)
                    .IsUnicode(true)
                    .IsRequired(true);

                entity
                    .Property(d => d.Specialty)
                    .HasMaxLength(100)
                    .IsUnicode(true)
                    .IsRequired(true);

                entity
                    .HasMany(v => v.Visitations)
                    .WithOne(d => d.Doctor)
                    .HasForeignKey(v => v.VisitationId);
            });
        }
    }
}
