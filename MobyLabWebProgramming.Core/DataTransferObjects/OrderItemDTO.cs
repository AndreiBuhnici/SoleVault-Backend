namespace MobyLabWebProgramming.Core.DataTransferObjects;

public class OrderItemDTO
{
    public Guid Id { get; set; }
    public ProductDTO Product { get; set; } = default!;
    public int Quantity { get; set; }
    public float Price { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}