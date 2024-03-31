using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Infrastructure.EntityConfigurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).IsRequired();
        builder.Property(o => o.UserId).IsRequired();
        builder.HasOne(o => o.User).WithMany(u => u.Orders).HasForeignKey(o => o.UserId).HasPrincipalKey(u => u.Id).IsRequired().OnDelete(DeleteBehavior.Cascade);
        builder.Property(o => o.OrderDate).IsRequired();
        builder.Property(o => o.DeliveryDate).IsRequired();
        builder.Property(o => o.ShippingAddress).IsRequired().HasMaxLength(500);
        builder.Property(o => o.PhoneNumber).IsRequired().HasMaxLength(20);
        builder.Property(o => o.Total).IsRequired();
        builder.Property(o => o.Status).IsRequired().HasMaxLength(100);
        builder.Property(e => e.CreatedAt)
            .IsRequired();
        builder.Property(e => e.UpdatedAt)
            .IsRequired();
    }
}