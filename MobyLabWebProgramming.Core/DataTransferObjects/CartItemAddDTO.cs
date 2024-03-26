namespace MobyLabWebProgramming.Core.DataTransferObjects;

public class CartItemAddDTO
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}