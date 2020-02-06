namespace P03_FootballBetting.Data.Configuration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Models;

    public class PlayerConfiguration : IEntityTypeConfiguration<Player>
    {
        public void Configure(EntityTypeBuilder<Player> entity)
        {
            entity
                .HasKey(p => p.PlayerId);

            entity
                .Property(p => p.Name)
                .HasMaxLength(60)
                .IsRequired(true)
                .IsUnicode(true);

            entity
                .Property(p => p.SquadNumber)
                .IsRequired(true);

            entity
                .HasOne(t => t.Team)
                .WithMany(p => p.Players)
                .HasForeignKey(t => t.TeamId);

            entity
                .HasOne(p => p.Position)
                .WithMany(p => p.Players)
                .HasForeignKey(p => p.PositionId);

            entity
                .Property(p => p.IsInjured)
                .IsRequired(true);

            entity
                .HasMany(g => g.PlayerStatistics)
                .WithOne(p => p.Player)
                .HasForeignKey(g => g.GameId);
        }
    }
}
