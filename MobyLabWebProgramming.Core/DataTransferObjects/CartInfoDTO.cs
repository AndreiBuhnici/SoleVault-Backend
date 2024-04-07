namespace MobyLabWebProgramming.Core.DataTransferObjects;

public class CartInfoDTO
{
    public Guid Id { get; set; }
    public int Size { get; set; }
    public float TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
