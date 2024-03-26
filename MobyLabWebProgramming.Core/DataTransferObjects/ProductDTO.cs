namespace MobyLabWebProgramming.Core.DataTransferObjects;

public class ProductDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public float Price { get; set; }
    public int Stock { get; set; }
    public int Size { get; set; }
    public string Color { get; set; } = default!;
    public string ImageUrl { get; set; } = default!;
    public CategoryDTO Category { get; set; } = default!;
    public UserDTO Owner { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}