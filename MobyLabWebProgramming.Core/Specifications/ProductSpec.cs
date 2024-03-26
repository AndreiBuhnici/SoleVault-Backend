using Ardalis.Specification;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

public class ProductSpec : BaseSpec<ProductSpec, Product>
{
    public ProductSpec(Guid id) : base(id)
    {
    }

    public ProductSpec(string name) 
    {
        Query.Where(e => e.Name == name);
    }
}