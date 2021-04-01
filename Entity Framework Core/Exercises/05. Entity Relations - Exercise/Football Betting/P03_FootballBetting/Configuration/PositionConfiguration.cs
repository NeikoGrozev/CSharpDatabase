namespace P03_FootballBetting.Data.Configuration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Models;

    public class PositionConfiguration : IEntityTypeConfiguration<Position>
    {
        public void Configure(EntityTypeBuilder<Position> entity)
        {
            entity
                .HasKey(p => p.PositionId);

            entity
                .Property(p => p.Name)
                .HasMaxLength(50)
                .IsRequired(true)
                .IsUnicode(false);

            entity
                .HasMany(p => p.Players)
                .WithOne(p => p.Position)
                .HasForeignKey(p => p.PositionId);
        }
    }
}
