using System.Net;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;
using MobyLabWebProgramming.Core.Enums;
using MobyLabWebProgramming.Core.Errors;
using MobyLabWebProgramming.Core.Requests;
using MobyLabWebProgramming.Core.Responses;
using MobyLabWebProgramming.Core.Specifications;
using MobyLabWebProgramming.Infrastructure.Database;
using MobyLabWebProgramming.Infrastructure.Repositories.Interfaces;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;

namespace MobyLabWebProgramming.Infrastructure.Services.Implementations;

public class CategoryService : ICategoryService
{
    private readonly IRepository<WebAppDatabaseContext> _repository;

    public CategoryService(IRepository<WebAppDatabaseContext> repository)
    {
        _repository = repository;
    }

    public async Task<ServiceResponse> AddCategory(CategoryAddDTO categoryAddDTO, UserDTO? requestingUser = null, CancellationToken cancellationToken = default)
    {
        if (requestingUser != null && requestingUser.Role != UserRoleEnum.Admin)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only the admin can add categories!", ErrorCodes.CannotAdd));
        }

        var result = await _repository.GetAsync(new CategorySpec(categoryAddDTO.Name), cancellationToken);

        if (result != null)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.BadRequest, "Category already exists!", ErrorCodes.CategoryAlreadyExists));
        }

        await _repository.AddAsync(new Category
        {
            Name = categoryAddDTO.Name,
            Description = categoryAddDTO.Description
        }, cancellationToken);

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> DeleteCategory(Guid id, UserDTO? requestingUser = null, CancellationToken cancellationToken = default)
    {
        if (requestingUser != null && requestingUser.Role != UserRoleEnum.Admin)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only the admin can delete categories!", ErrorCodes.CannotAdd));
        }

        var category = await _repository.GetAsync(new CategorySpec(id), cancellationToken);

        if (category == null)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.NotFound, "Category not found!", ErrorCodes.EntityNotFound));
        }

        await _repository.DeleteAsync<Category>(id, cancellationToken);

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse<PagedResponse<CategoryDTO>>> GetCategories(PaginationSearchQueryParams pagination, CancellationToken cancellationToken = default)
    {
        var result = await _repository.PageAsync(pagination, new CategoryProjectionSpec(pagination.Search), cancellationToken);

        return ServiceResponse<PagedResponse<CategoryDTO>>.ForSuccess(result);
    }

    public async Task<ServiceResponse<CategoryDTO>> GetCategory(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _repository.GetAsync(new CategoryProjectionSpec(id), cancellationToken);

        return result != null ? 
            ServiceResponse<CategoryDTO>.ForSuccess(result) : 
            ServiceResponse<CategoryDTO>.FromError(new(HttpStatusCode.NotFound, "Category not found!", ErrorCodes.EntityNotFound));
    }

    public async Task<ServiceResponse> UpdateCategory(CategoryUpdateDTO categoryUpdateDTO, UserDTO? requestingUser = null, CancellationToken cancellationToken = default)
    {
        if (requestingUser != null && requestingUser.Role != UserRoleEnum.Admin)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only the admin can modify categories!", ErrorCodes.CannotAdd));
        }

        var entity = await _repository.GetAsync(new CategorySpec(categoryUpdateDTO.Id), cancellationToken);

        if (entity != null)
        {
            entity.Name = categoryUpdateDTO.Name ?? entity.Name;
            entity.Description = categoryUpdateDTO.Description ?? entity.Description;

            await _repository.UpdateAsync(entity, cancellationToken);
        }
        else
        {
            return ServiceResponse.FromError(new(HttpStatusCode.NotFound, "Category not found!", ErrorCodes.EntityNotFound));
        }

        return ServiceResponse.ForSuccess();
    }
}