using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Requests;
using MobyLabWebProgramming.Core.Responses;

namespace MobyLabWebProgramming.Infrastructure.Services.Interfaces;

public interface IProductService
{
    public Task<ServiceResponse<PagedResponse<ProductDTO>>> GetProducts(PaginationSearchQueryParams pagination, CancellationToken cancellationToken = default);
    public Task<ServiceResponse<ProductDTO>> GetProduct(Guid id, CancellationToken cancellationToken = default);
    public Task<ServiceResponse> AddProduct(ProductAddDTO productAddDTO, UserDTO requestingUser, CancellationToken cancellationToken = default);
    public Task<ServiceResponse> UpdateProduct(ProductUpdateDTO productUpdateDTO, UserDTO requestingUser, CancellationToken cancellationToken = default);
    public Task<ServiceResponse> DeleteProduct(Guid id, UserDTO requestingUserl, CancellationToken cancellationToken = default);
}