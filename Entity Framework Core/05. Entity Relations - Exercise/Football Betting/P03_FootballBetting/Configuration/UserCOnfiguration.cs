namespace P03_FootballBetting.Data.Configuration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Models;

    public class UserCOnfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> entity)
        {
            entity
                .HasKey(u => u.UserId);

            entity
                .Property(u => u.Username)
                .HasMaxLength(60)
                .IsRequired(true)
                .IsUnicode(false);

            entity
                .Property(u => u.Password)
                .HasMaxLength(60)
                .IsRequired(true)
                .IsUnicode(true);

            entity
                .Property(u => u.Email)
                .HasMaxLength(100)
                .IsRequired(true)
                .IsUnicode(false);

            entity
                .Property(u => u.Name)
                .HasMaxLength(60)
                .IsRequired(true)
                .IsRequired(true);

            entity
                .Property(u => u.Balance)
                .IsRequired(true);

            entity
                .HasMany(u => u.Bets)
                .WithOne(b => b.User)
                .HasForeignKey(u => u.UserId);
        }
    }
}
