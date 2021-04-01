namespace P03_FootballBetting.Data.Configuration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Models;

    public class GameConfiguration : IEntityTypeConfiguration<Game>
    {
        public void Configure(EntityTypeBuilder<Game> entity)
        {
            entity
                 .HasKey(g => g.GameId);

            entity
                .HasOne(t => t.HomeTeam)
                .WithMany(g => g.HomeGames)
                .HasForeignKey(t => t.HomeTeamId);

            entity
                .HasOne(t => t.AwayTeam)
                .WithMany(g => g.AwayGames)
                .HasForeignKey(t => t.AwayTeamId);

            entity
                .Property(g => g.HomeTeamGoals)
                .IsRequired(true);

            entity
                .Property(g => g.AwayTeamGoals)
                .IsRequired(true);

            entity
                .Property(g => g.DateTime)
                .HasColumnType("DATETIME2")
                .IsRequired(true);

            entity
                .Property(g => g.HomeTeamBetRate)
                .IsRequired(true);

            entity
                .Property(g => g.AwayTeamBetRate)
                .IsRequired(true);

            entity
                .Property(g => g.DrawBetRate)
                .IsRequired(true);

            entity
                .Property(g => g.Result)
                .HasMaxLength(7)
                .IsRequired(true);

            entity
                .HasMany(b => b.Bets)
                .WithOne(g => g.Game)
                .HasForeignKey(b => b.BetId);

            entity
                .HasMany(g => g.PlayerStatistics)
                .WithOne(ps => ps.Game)
                .HasForeignKey(g => g.GameId);

        }
    }
}
