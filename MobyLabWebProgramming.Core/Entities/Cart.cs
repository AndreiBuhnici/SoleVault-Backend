namespace MobyLabWebProgramming.Core.Entities;

public class Cart : BaseEntity
{
    public int Size { get; set; }
    public float TotalPrice { get; set; }
    public User User { get; set; } = default!;
    public ICollection<CartItem> CartItems { get; set; } = default!;
}
