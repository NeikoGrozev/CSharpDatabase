namespace P03_FootballBetting.Data.Configuration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Models;

    public class TownConfiguration : IEntityTypeConfiguration<Town>
    {
        public void Configure(EntityTypeBuilder<Town> entity)
        {
            entity
                .HasKey(t => t.TownId);

            entity
                .Property(t => t.Name)
                .HasMaxLength(100)
                .IsRequired(true)
                .IsUnicode(false);

            entity
                .HasOne(c => c.Country)
                .WithMany(t => t.Towns)
                .HasForeignKey(c => c.CountryId);

            entity
                .HasMany(t => t.Teams)
                .WithOne(t => t.Town)
                .HasForeignKey(t => t.TownId);
        }
    }
}
