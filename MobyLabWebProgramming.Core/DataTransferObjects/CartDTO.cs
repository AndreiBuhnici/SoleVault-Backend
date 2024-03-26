namespace MobyLabWebProgramming.Core.DataTransferObjects;

public class CartDTO
{
    public Guid Id { get; set; }
    public ICollection<CartItemDTO> CartItems { get; set; } = new List<CartItemDTO>();
    public int Size { get; set; }
    public float TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
