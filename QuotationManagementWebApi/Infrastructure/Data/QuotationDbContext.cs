using Microsoft.EntityFrameworkCore;
using QuotationManagementWebApi.Entities;

namespace QuotationManagementWebApi.Infrastructure.Data
{
    public class QuotationDbContext : DbContext
    {
        public QuotationDbContext(DbContextOptions<QuotationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Quotation> Quotations { get; set; }
        public DbSet<QuotationLineItem> QuotationLineItems { get; set; }
        public DbSet<QuotationTemplate> QuotationTemplates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Quotation>()
                .HasMany(q => q.LineItems)
                .WithOne(li => li.Quotation)
                .HasForeignKey(li => li.QuoteId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Quotation>()
                .HasIndex(q => q.QuoteNumber)
                .IsUnique();

            modelBuilder.Entity<Quotation>()
                .HasIndex(q => q.Status);

            modelBuilder.Entity<Quotation>()
                .HasIndex(q => q.CreatedBy);

            modelBuilder.Entity<Quotation>()
                .HasQueryFilter(q => !q.IsDeleted);

            modelBuilder.Entity<Quotation>()
                .Property(q => q.SubTotal)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Quotation>()
                .Property(q => q.TaxAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Quotation>()
                .Property(q => q.DiscountAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Quotation>()
                .Property(q => q.GrandTotal)
                .HasPrecision(18, 2);

            modelBuilder.Entity<QuotationLineItem>()
                .Property(li => li.UnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<QuotationLineItem>()
                .Property(li => li.Discount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<QuotationLineItem>()
                .Property(li => li.LineTotal)
                .HasPrecision(18, 2);
        }
    }
}