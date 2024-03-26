using System.Linq.Expressions;
using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

public sealed class OrderItemProjectionSpec : BaseSpec<OrderItemProjectionSpec, OrderItem, OrderItemDTO>
{
    protected override Expression<Func<OrderItem, OrderItemDTO>> Spec => e => new()
    {
        Id = e.Id,
        Product = new ProductDTO
        {
            Id = e.Product.Id,
            Name = e.Product.Name,
            Description = e.Product.Description,
            Price = e.Product.Price,
            Stock = e.Product.Stock,
            Size = e.Product.Size,
            Color = e.Product.Color,
            ImageUrl = e.Product.ImageUrl,
            Category = new CategoryDTO
            {
                Id = e.Product.Category.Id,
                Name = e.Product.Category.Name,
                Description = e.Product.Category.Description
            },
            Owner = new UserDTO
            {
                Id = e.Product.Owner.Id,
                Email = e.Product.Owner.Email,
                Name = e.Product.Owner.Name,
                Role = e.Product.Owner.Role
            },
            CreatedAt = e.Product.CreatedAt,
            UpdatedAt = e.Product.UpdatedAt
        },
        Quantity = e.Quantity,
        Price = e.Price,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt
    };

    public OrderItemProjectionSpec(Guid orderId)
    {
        Query.Where(e => e.OrderId == orderId);
    }
}