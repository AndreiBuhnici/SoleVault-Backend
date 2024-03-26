using System.Linq.Expressions;
using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

public sealed class ProductProjectionSpec : BaseSpec<ProductProjectionSpec, Product, ProductDTO>
{
    protected override Expression<Func<Product, ProductDTO>> Spec => e => new()
    {
        Id = e.Id,
        Name = e.Name,
        Description = e.Description,
        Price = e.Price,
        Stock = e.Stock,
        Size = e.Size,
        Color = e.Color,
        ImageUrl = e.ImageUrl,
        Category = new()
        {
            Id = e.Category.Id,
            Name = e.Category.Name,
            Description = e.Category.Description
        },
        Owner = new()
        {
            Id = e.Owner.Id,
            Email = e.Owner.Email,
            Name = e.Owner.Name,
            Role = e.Owner.Role
        },
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt
    };

    public ProductProjectionSpec(Guid id) : base(id)
    {
    }

    public ProductProjectionSpec(string? search)
    {
        search = !string.IsNullOrWhiteSpace(search) ? search.Trim() : null;

        if (search == null)
        {
            return;
        }

        var searchExpr = $"%{search.Replace(" ", "%")}%";

        Query.Where(e => EF.Functions.ILike(e.Name, searchExpr) ||
                                       EF.Functions.ILike(e.Description, searchExpr) ||
                                       EF.Functions.ILike(e.Category.Name, searchExpr) ||
                                       EF.Functions.ILike(e.Owner.Name, searchExpr));
    }
}
