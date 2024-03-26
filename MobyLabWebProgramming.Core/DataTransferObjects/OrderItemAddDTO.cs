namespace MobyLabWebProgramming.Core.DataTransferObjects;

public class OrderItemAddDTO
{
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public float Price { get; set; }
}
