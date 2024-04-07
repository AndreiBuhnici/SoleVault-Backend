using System.Linq.Expressions;
using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

public sealed class CartItemProjectionSpec : BaseSpec<CartItemProjectionSpec, CartItem, CartItemDTO>
{
    protected override Expression<Func<CartItem, CartItemDTO>> Spec => e => new()
    {
        Id = e.Id,
        Product = new()
        {
            Id = e.Product.Id,
            Name = e.Product.Name,
            Description = e.Product.Description,
            Price = e.Product.Price,
            Stock = e.Product.Stock,
            ImageUrl = e.Product.ImageUrl,
            Category = new()
            {
                Id = e.Product.Category.Id,
                Name = e.Product.Category.Name,
                Description = e.Product.Category.Description,
                CreatedAt = e.Product.Category.CreatedAt,
                UpdatedAt = e.Product.Category.UpdatedAt
            },
            Owner = new()
            {
                Id = e.Product.Owner.Id,
                Email = e.Product.Owner.Email,
                Name = e.Product.Owner.Name,
                Role = e.Product.Owner.Role,
            },
        },
        Quantity = e.Quantity,
        Price = e.Price,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt
    };

    public CartItemProjectionSpec(Guid Id, string? search)
    {
        if (search == "Cart")
        {
            Query.Where(e => e.CartId == Id);
            return;
        }

        if (search == "Product")
        {
            Query.Where(e => e.ProductId == Id);
            return;
        }

        search = !string.IsNullOrWhiteSpace(search) ? search.Trim() : null;

        if (search == null)
        {
            Query.Where(e => e.CartId == Id);
            return;
        }

        var searchExpr = $"%{search.Replace(" ", "%")}%";

        Query.Where(e => e.CartId == Id).Where(e => EF.Functions.ILike(e.Product.Name, searchExpr) ||
                                       EF.Functions.ILike(e.Product.Description, searchExpr) ||
                                       EF.Functions.ILike(e.Product.Category.Name, searchExpr) ||
                                       EF.Functions.ILike(e.Product.Owner.Name, searchExpr) ||
                                       EF.Functions.ILike(e.Product.Color, searchExpr) ||
                                       EF.Functions.ILike(e.Product.Size.ToString(), searchExpr));
    }

    public CartItemProjectionSpec(Guid productId, Guid cartId)
    {
        Query.Where(e => e.ProductId == productId && e.CartId == cartId);
    }
}