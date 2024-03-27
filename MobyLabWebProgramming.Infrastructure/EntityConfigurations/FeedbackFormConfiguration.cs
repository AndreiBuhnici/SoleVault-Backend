using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Infrastructure.EntityConfigurations;

public class FeedbackFormConfiguration : IEntityTypeConfiguration<FeedbackForm>
{
    public void Configure(EntityTypeBuilder<FeedbackForm> builder)
    {
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id).IsRequired();
        builder.Property(f => f.Feedback).IsRequired();
        builder.Property(f => f.OverallRating).IsRequired();
        builder.Property(f => f.DeliveryRating).IsRequired();
        builder.Property(f => f.FavoriteFeatures).IsRequired();

        builder.HasOne(e => e.User)
            .WithOne(u => u.FeedbackForm)
            .HasForeignKey<User>(e => e.FeedbackFormId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
    }
}