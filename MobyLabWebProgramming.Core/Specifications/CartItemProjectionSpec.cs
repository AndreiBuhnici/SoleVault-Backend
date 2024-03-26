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
                Name = e.Product.Category.Name
            }
        },
        Quantity = e.Quantity,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt
    };

    public CartItemProjectionSpec(Guid cartId)
    {
        Query.Where(e => e.CartId == cartId);
    }

    public CartItemProjectionSpec(Guid productId, Guid cartId)
    {
        Query.Where(e => e.ProductId == productId && e.CartId == cartId);
    }
}