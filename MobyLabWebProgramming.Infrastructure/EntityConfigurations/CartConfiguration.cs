using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Infrastructure.EntityConfigurations;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).IsRequired();
        builder.Property(c => c.Size).IsRequired();
        builder.Property(c => c.TotalPrice).IsRequired();
        builder.Property(e => e.CreatedAt)
            .IsRequired();
        builder.Property(e => e.UpdatedAt)
            .IsRequired();

        builder.HasOne(e => e.User)
            .WithOne(u => u.Cart)
            .HasForeignKey<User>(e => e.CartId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

