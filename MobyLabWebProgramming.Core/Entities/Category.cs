﻿namespace MobyLabWebProgramming.Core.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public ICollection<Product> Products { get; set; } = default!;
}