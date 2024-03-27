namespace MobyLabWebProgramming.Core.Entities;

public class FeedbackForm : BaseEntity
{
    public User User { get; set; } = default!;
    public string Feedback { get; set; } = default!;
    public int OverallRating { get; set; }
    public int DeliveryRating { get; set; }
    public string FavoriteFeatures { get; set; } = default!;
}