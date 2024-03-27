namespace MobyLabWebProgramming.Core.DataTransferObjects;

public class FeedbackFormDTO
{
    public Guid Id { get; set; }
    public string Feedback { get; set; } = default!;
    public int OverallRating { get; set; }
    public int DeliveryRating { get; set; }
    public string FavoriteFeatures { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}