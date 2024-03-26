namespace MobyLabWebProgramming.Core.DataTransferObjects;

public class ProductAddDTO
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public float Price { get; set; }
    public int Stock { get; set; }
    public int Size { get; set; }
    public string Color { get; set; } = default!;
    public string ImageUrl { get; set; } = default!;
    public Guid CategoryId { get; set; }
}