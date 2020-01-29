namespace P03_SalesDatabase.Data
{
    using Microsoft.EntityFrameworkCore;
    using P03_SalesDatabase.Data.Models;

    public class SalesContext : DbContext
    {
        public SalesContext()
        {

        }

        public SalesContext(DbContextOptions options)
            : base(options)
        {

        }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Sale> Sales { get; set; }

        public DbSet<Store> Stores { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=NEIKO\\SQLEXPRESS; " +
                    "Database=Sales; Integrated Security=true");
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfiguringCustomerEntity(modelBuilder);
            ConfiguringProductEntity(modelBuilder);
            ConfiguringStoreEntity(modelBuilder);
            ConfiguringSaleEntity(modelBuilder);
        }

        private void ConfiguringCustomerEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(c => c.CustomerId);

                entity
                    .Property(c => c.Name)
                    .HasMaxLength(100)
                    .IsUnicode(true)
                    .IsRequired(true);

                entity
                    .Property(c => c.Email)
                    .HasMaxLength(80)
                    .IsUnicode(false)
                    .IsRequired(true);

                entity
                    .Property(c => c.CreditCardNumber)
                    .IsRequired(true);

                entity
                    .HasMany(s => s.Sales)
                    .WithOne(c => c.Customer)
                    .HasForeignKey(s => s.SaleId);
            });
        }

        private void ConfiguringProductEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.ProductId);

                entity
                    .Property(p => p.Name)
                    .HasMaxLength(50)
                    .IsUnicode(true)
                    .IsRequired(true);

                entity
                    .Property(p => p.Quantity)
                    .IsRequired(true);

                entity
                    .Property(p => p.Price)
                    .IsRequired(true);

                entity
                    .Property(p => p.Description)
                    .HasMaxLength(250)
                    .IsRequired(false)
                    .IsUnicode(true)
                    .HasDefaultValue("No description");

                entity
                    .HasMany(s => s.Sales)
                    .WithOne(p => p.Product)
                    .HasForeignKey(s => s.SaleId);
            });
        }

        private void ConfiguringStoreEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Store>(entity =>
            {
                entity.HasKey(s => s.StoreId);

                entity
                    .Property(s => s.Name)
                    .HasMaxLength(80)
                    .IsUnicode(true)
                    .IsRequired(true);

                entity
                    .HasMany(s => s.Sales)
                    .WithOne(s => s.Store)
                    .HasForeignKey(s => s.SaleId);
            });
        }

        private void ConfiguringSaleEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sale>(entity =>
            {
                entity.HasKey(s => s.SaleId);

                entity
                    .Property(s => s.Date)
                    .HasColumnType("DATETIME2")
                    .HasDefaultValueSql("GETDATE()");

                entity
                    .HasOne(p => p.Product)
                    .WithMany(s => s.Sales)
                    .HasForeignKey(p => p.ProductId);

                entity
                    .HasOne(c => c.Customer)
                    .WithMany(s => s.Sales)
                    .HasForeignKey(c => c.CustomerId);

                entity
                    .HasOne(s => s.Store)
                    .WithMany(s => s.Sales)
                    .HasForeignKey(s => s.StoreId);
            });
        }

    }
}
