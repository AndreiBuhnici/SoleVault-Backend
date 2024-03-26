using System.Linq.Expressions;
using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

public sealed class CategoryProjectionSpec : BaseSpec<CategoryProjectionSpec, Category, CategoryDTO>
{
    protected override Expression<Func<Category, CategoryDTO>> Spec => e => new()
    {
        Id = e.Id,
        Name = e.Name,
        Description = e.Description,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt
    };

    public CategoryProjectionSpec(Guid id) : base(id)
    {
    }

    public CategoryProjectionSpec(string? search)
    {
        search = !string.IsNullOrWhiteSpace(search) ? search.Trim() : null;

        if (search == null)
        {
            return;
        }

        var searchExpr = $"%{search.Replace(" ", "%")}%";

        Query.Where(e => EF.Functions.ILike(e.Name, searchExpr) ||
                                EF.Functions.ILike(e.Description, searchExpr));
    }
}