using System.Linq.Expressions;
using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

/// <summary>
/// This is a specification to filter the user entities and map it to and UserDTO object via the constructors.
/// Note how the constructors call the base class's constructors. Also, this is a sealed class, meaning it cannot be further derived.
/// </summary>
public sealed class UserProjectionSpec : BaseSpec<UserProjectionSpec, User, UserDTO>
{
    /// <summary>
    /// This is the projection/mapping expression to be used by the base class to get UserDTO object from the database.
    /// </summary>
    protected override Expression<Func<User, UserDTO>> Spec => e => new()
    {
        Id = e.Id,
        Email = e.Email,
        Name = e.Name,
        Role = e.Role,
        Cart = e.Cart != null ? new()
        {
            Id = e.Cart.Id,
            CartItems = e.Cart.CartItems.Select(ci => new CartItemDTO
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
            Size = e.Cart.Size,
            TotalPrice = e.Cart.TotalPrice,
            CreatedAt = e.Cart.CreatedAt,
            UpdatedAt = e.Cart.UpdatedAt,
        } : null,
    };

    public UserProjectionSpec(bool orderByCreatedAt = true) : base(orderByCreatedAt)
    {
    }

    public UserProjectionSpec(Guid id) : base(id)
    {
    }

    public UserProjectionSpec(string? search)
    {
        search = !string.IsNullOrWhiteSpace(search) ? search.Trim() : null;

        if (search == null)
        {
            return;
        }

        var searchExpr = $"%{search.Replace(" ", "%")}%";

        Query.Where(e => EF.Functions.ILike(e.Name, searchExpr)); // This is an example on who database specific expressions can be used via C# expressions.
                                                                                           // Note that this will be translated to the database something like "where user.Name ilike '%str%'".
    }
}
