namespace P03_FootballBetting.Data.Configuration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Models;

    public class ColorConfiguration : IEntityTypeConfiguration<Color>
    {
        public void Configure(EntityTypeBuilder<Color> entity)
        {
            entity
                .HasKey(c => c.ColorId);

            entity
                .Property(c => c.Name)
                .HasMaxLength(50)
                .IsRequired(true)
                .IsUnicode(false);

            entity
                .HasMany(t => t.PrimaryKitTeams)
                .WithOne(c => c.PrimaryKitColor)
                .HasForeignKey(t => t.PrimaryKitColorId);

            entity
                .HasMany(t => t.SecondaryKitTeams)
                .WithOne(c => c.SecondaryKitColor)
                .HasForeignKey(t => t.SecondaryKitColorId);
        }
    }
}
