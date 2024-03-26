using Ardalis.Specification;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

public sealed class CategorySpec : BaseSpec<CategorySpec, Category>
{
    public CategorySpec(Guid id) : base(id)
    {
    }

    public CategorySpec(string name)
    {
        Query.Where(e => e.Name == name);
    }
}