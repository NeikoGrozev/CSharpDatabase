namespace P03_FootballBetting.Data.Configuration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Models;

    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> entity)
        {
            entity
                 .HasKey(c => c.CountryId);

            entity
                .Property(c => c.Name)
                .HasMaxLength(100)
                .IsRequired(true)
                .IsUnicode(false);

            entity
                .HasMany(t => t.Towns)
                .WithOne(c => c.Country)
                .HasForeignKey(t => t.CountryId);
        }
    }
}
