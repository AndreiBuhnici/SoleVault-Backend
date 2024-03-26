using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Requests;
using MobyLabWebProgramming.Core.Responses;

namespace MobyLabWebProgramming.Infrastructure.Services.Interfaces;

public interface ICategoryService
{
    public Task<ServiceResponse<PagedResponse<CategoryDTO>>> GetCategories(PaginationSearchQueryParams pagination, CancellationToken cancellationToken = default);
    public Task<ServiceResponse<CategoryDTO>> GetCategory(Guid id, CancellationToken cancellationToken = default);
    public Task<ServiceResponse> AddCategory(CategoryAddDTO categoryAddDTO, UserDTO? requestingUser = default, CancellationToken cancellationToken = default);
    public Task<ServiceResponse> UpdateCategory(CategoryUpdateDTO categoryUpdateDTO, UserDTO? requestingUser = default, CancellationToken cancellationToken = default);
    public Task<ServiceResponse> DeleteCategory(Guid id, UserDTO? requestingUser = default, CancellationToken cancellationToken = default);
}