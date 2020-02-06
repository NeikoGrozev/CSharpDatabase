namespace P03_FootballBetting.Data.Configuration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Models;

    public class TeamConfiguration : IEntityTypeConfiguration<Team>
    {
        public void Configure(EntityTypeBuilder<Team> entity)
        {
            entity
                .HasKey(t => t.TeamId);

            entity
                .Property(t => t.Name)
                .HasMaxLength(100)
                .IsRequired(true)
                .IsUnicode(true);

            entity
                .Property(t => t.LogoUrl)
                .HasMaxLength(300)
                .IsRequired(false)
                .IsUnicode(false);

            entity
                .Property(t => t.Initials)
                .HasMaxLength(10)
                .IsRequired(false)
                .IsUnicode(false);

            entity
                .Property(t => t.Budget)
                .IsRequired(true);

            entity
                .HasOne(c => c.PrimaryKitColor)
                .WithMany(t => t.PrimaryKitTeams)
                .HasForeignKey(c => c.PrimaryKitColorId);

            entity
                .HasOne(c => c.SecondaryKitColor)
                .WithMany(t => t.SecondaryKitTeams)
                .HasForeignKey(c => c.SecondaryKitColorId);

            entity
                .HasOne(t => t.Town)
                .WithMany(t => t.Teams)
                .HasForeignKey(t => t.TownId);

            entity
                .HasMany(t => t.HomeGames)
                .WithOne(g => g.HomeTeam)
                .HasForeignKey(t => t.HomeTeamId);

            entity
                .HasMany(t => t.AwayGames)
                .WithOne(g => g.AwayTeam)
                .HasForeignKey(t => t.AwayTeamId);

            entity
                .HasMany(t => t.Players)
                .WithOne(p => p.Team)
                .HasForeignKey(t => t.TeamId);
        }
    }
}
