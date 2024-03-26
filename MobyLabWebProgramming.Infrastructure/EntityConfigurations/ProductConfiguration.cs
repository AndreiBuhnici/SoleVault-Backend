using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Infrastructure.EntityConfigurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).IsRequired();
        builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Description).IsRequired().HasMaxLength(500);
        builder.Property(p => p.Price).IsRequired();
        builder.Property(p => p.Stock).IsRequired();
        builder.Property(p => p.Size).IsRequired();
        builder.Property(p => p.Color).IsRequired().HasMaxLength(50);
        builder.Property(p => p.ImageUrl).IsRequired().HasMaxLength(500);
        builder.Property(p => p.CategoryId).IsRequired();
        builder.HasOne(p => p.Category).WithMany(c => c.Products).HasForeignKey(p => p.CategoryId).IsRequired().OnDelete(DeleteBehavior.Cascade);
        builder.Property(p => p.OwnerId).IsRequired();
        builder.HasOne(p => p.Owner).WithMany(u => u.Products).HasForeignKey(p => p.OwnerId).IsRequired().OnDelete(DeleteBehavior.Cascade);
        builder.Property(e => e.CreatedAt)
            .IsRequired();
        builder.Property(e => e.UpdatedAt)
            .IsRequired();
    }
}