namespace MobyLabWebProgramming.Core.DataTransferObjects;

public class FeedBackFormAddDTO
{
    public string Feedback { get; set; } = default!;
    public int OverallRating { get; set; }
    public int DeliveryRating { get; set; }
    public string FavoriteFeatures { get; set; } = default!;
}