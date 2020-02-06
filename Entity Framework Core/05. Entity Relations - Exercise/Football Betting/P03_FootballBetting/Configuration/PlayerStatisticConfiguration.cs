namespace P03_FootballBetting.Data.Configuration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Models;

    public class PlayerStatisticConfiguration : IEntityTypeConfiguration<PlayerStatistic>
    {
        public void Configure(EntityTypeBuilder<PlayerStatistic> entity)
        {
            entity
                .HasKey(ps => new { ps.GameId, ps.PlayerId });

            entity
                .HasOne(g => g.Game)
                .WithMany(ps => ps.PlayerStatistics)
                .HasForeignKey(g => g.GameId);

            entity
                .HasOne(p => p.Player)
                .WithMany(ps => ps.PlayerStatistics)
                .HasForeignKey(p => p.PlayerId);

            entity
                .Property(ps => ps.ScoredGoals)
                .IsRequired(true);

            entity
                .Property(ps => ps.Assists)
                .IsRequired(true);

            entity
                .Property(ps => ps.MinutesPlayed)
                .IsRequired(true);
        }
    }
}
