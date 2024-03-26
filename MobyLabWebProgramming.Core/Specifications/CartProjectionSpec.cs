using System.Linq.Expressions;
using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

public sealed class CartProjectionSpec : BaseSpec<CartProjectionSpec, Cart, CartDTO>
{
    protected override Expression<Func<Cart, CartDTO>> Spec => e => new()
    {
        Id = e.Id,
        CartItems = e.CartItems.Select(ci => new CartItemDTO
        {
            Id = ci.Id,
            Product = new ProductDTO
            {
                Id = ci.Product.Id,
                Name = ci.Product.Name,
                Description = ci.Product.Description,
                Price = ci.Product.Price,
                Stock = ci.Product.Stock,
                Size = ci.Product.Size,
                Color = ci.Product.Color,
                ImageUrl = ci.Product.ImageUrl,
                Category = new CategoryDTO
                {
                    Id = ci.Product.Category.Id,
                    Name = ci.Product.Category.Name,
                    Description = ci.Product.Category.Description
                },
                Owner = new UserDTO
                {
                    Id = ci.Product.Owner.Id,
                    Email = ci.Product.Owner.Email,
                    Name = ci.Product.Owner.Name,
                    Role = ci.Product.Owner.Role
                },
                CreatedAt = ci.Product.CreatedAt,
                UpdatedAt = ci.Product.UpdatedAt
            },
            Quantity = ci.Quantity,
            Price = ci.Price,
            CreatedAt = ci.CreatedAt,
            UpdatedAt = ci.UpdatedAt
        }).ToList(),
        Size = e.Size,
        TotalPrice = e.TotalPrice,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt
    };

    public CartProjectionSpec(Guid id) : base(id)
    {
    }
}