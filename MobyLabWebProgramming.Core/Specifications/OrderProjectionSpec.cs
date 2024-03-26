using System.Linq.Expressions;
using Ardalis.Specification;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

public sealed class OrderProjectionSpec : BaseSpec<OrderProjectionSpec, Order, OrderDTO>
{
    protected override Expression<Func<Order, OrderDTO>> Spec => e => new()
    {
        Id = e.Id,
        OrderItems = e.OrderItems.Select(oi => new OrderItemDTO
        {
            Id = oi.Id,
            Product = new ProductDTO
            {
                Id = oi.Product.Id,
                Name = oi.Product.Name,
                Description = oi.Product.Description,
                Price = oi.Product.Price,
                Stock = oi.Product.Stock,
                Size = oi.Product.Size,
                Color = oi.Product.Color,
                ImageUrl = oi.Product.ImageUrl,
                Category = new CategoryDTO
                {
                    Id = oi.Product.Category.Id,
                    Name = oi.Product.Category.Name,
                    Description = oi.Product.Category.Description
                },
                Owner = new UserDTO
                {
                    Id = oi.Product.Owner.Id,
                    Email = oi.Product.Owner.Email,
                    Name = oi.Product.Owner.Name,
                    Role = oi.Product.Owner.Role
                },
                CreatedAt = oi.Product.CreatedAt,
                UpdatedAt = oi.Product.UpdatedAt
            },
            Quantity = oi.Quantity,
            Price = oi.Price,
            CreatedAt = oi.CreatedAt,
            UpdatedAt = oi.UpdatedAt
        }).ToList(),
        OrderDate = e.OrderDate,
        DeliveryDate = e.DeliveryDate,
        ShippingAddress = e.ShippingAddress,
        Status = e.Status,
        Total = e.Total,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt
    };

    public OrderProjectionSpec(Guid userId)
    {
        Query.Where(e => e.UserId == userId);
    }
}