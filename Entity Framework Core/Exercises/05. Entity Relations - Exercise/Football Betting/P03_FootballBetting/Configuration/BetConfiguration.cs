namespace P03_FootballBetting.Data.Configurating
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Models;

    public class BetConfiguration : IEntityTypeConfiguration<Bet>
    {
        public void Configure(EntityTypeBuilder<Bet> entity)
        {
            entity
                .Property(b => b.BetId);

            entity
                .Property(b => b.Amount)
                .IsRequired(true);

            entity
                .Property(b => b.Prediction)
                .HasMaxLength(100)
                .IsRequired(true)
                .IsUnicode(false);

            entity
                .Property(b => b.DateTime)
                .HasColumnType("DATETIME2")
                .IsRequired(true);

            entity
                .HasOne(b => b.User)
                .WithMany(u => u.Bets)
                .HasForeignKey(b => b.UserId);

            entity
                .HasOne(b => b.Game)
                .WithMany(g => g.Bets)
                .HasForeignKey(b => b.GameId);
        }
    }
}
