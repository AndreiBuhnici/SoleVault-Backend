namespace MobyLabWebProgramming.Core.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public float Price { get; set; } = default!;
    public int Stock { get; set; } = default!;
    public int Size { get; set; } = default!;
    public string Color { get; set; } = default!;
    public string ImageUrl { get; set; } = default!;
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = default!;
    public Guid OwnerId { get; set; }
    public User Owner { get; set; } = default!;
}
