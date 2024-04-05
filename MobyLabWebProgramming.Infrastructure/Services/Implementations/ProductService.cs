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

public class ProductService : IProductService
{
    private readonly IRepository<WebAppDatabaseContext> _repository;
    private readonly IUserService _userService;
    private readonly ICartItemService _cartItemService;
    private readonly IOrderItemService _orderItemService;

    public ProductService(IRepository<WebAppDatabaseContext> repository, IUserService userService, ICartItemService cartItemService, IOrderItemService orderItemService)
    {
        _repository = repository;
        _userService = userService;
        _cartItemService = cartItemService;
        _orderItemService = orderItemService;
    }

    public async Task<ServiceResponse> AddProduct(ProductAddDTO productAddDTO, UserDTO requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser.Role != UserRoleEnum.Personnel)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only personnel can add products!", ErrorCodes.CannotAdd));
        }

        var category = await _repository.GetAsync<Category>(productAddDTO.CategoryId, cancellationToken);

        if (category == null)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.BadRequest, "Category not found!", ErrorCodes.EntityNotFound));
        }

        var result = await _repository.GetAsync(new ProductSpec(productAddDTO.Name), cancellationToken);

        if (result != null && result.Size == productAddDTO.Size && result.Color == productAddDTO.Color)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.BadRequest, "Product already exists!", ErrorCodes.ProductAlreadyExists));
        }

        if (productAddDTO.Stock < 0)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.BadRequest, "Stock cannot be negative!", ErrorCodes.InvalidStock));
        }

        if (productAddDTO.Price < 0)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.BadRequest, "Price cannot be negative!", ErrorCodes.InvalidPrice));
        }

        if (productAddDTO.Size < 0)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.BadRequest, "Size cannot be negative!", ErrorCodes.InvalidSize));
        }

        await _repository.AddAsync(new Product
        {
            Name = productAddDTO.Name,
            Description = productAddDTO.Description,
            Price = productAddDTO.Price,
            Stock = productAddDTO.Stock,
            Size = productAddDTO.Size,
            Color = productAddDTO.Color,
            ImageUrl = productAddDTO.ImageUrl,
            CategoryId = productAddDTO.CategoryId,
            OwnerId = requestingUser.Id
        }, cancellationToken);

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> DeleteProduct(Guid id, UserDTO requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser.Role != UserRoleEnum.Personnel)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only personnel can add products!", ErrorCodes.CannotAdd));
        }

        var product = await _repository.GetAsync<Product>(id, cancellationToken);

        if (product == null)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.BadRequest, "Product not found!", ErrorCodes.EntityNotFound));
        }

        var owner = await _userService.GetUser(product.OwnerId, cancellationToken);

        if (owner.Result == null)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.BadRequest, "Owner not found!", ErrorCodes.EntityNotFound));
        }

        if (owner.Result.Id != requestingUser.Id)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "You are not the owner of this product!", ErrorCodes.NotOwner));
        }

        var cartItems = await _cartItemService.GetCartItemsByProductId(id, cancellationToken);
        var orderItems = await _orderItemService.GetOrderItemsByProductId(id, cancellationToken);

        if ((cartItems.Result != null && cartItems.Result.Count > 0) || (orderItems.Result != null && orderItems.Result.Count > 0))
        {
            await UpdateProduct(new ProductUpdateDTO
            (
                Id : id,
                Stock : 0
            ), requestingUser, cancellationToken);
        } else {
            await _repository.DeleteAsync<Product>(id, cancellationToken);
        }

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse<ProductDTO>> GetProduct(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _repository.GetAsync(new ProductProjectionSpec(id, "Product"), cancellationToken);

        return product != null ?
            ServiceResponse<ProductDTO>.ForSuccess(product) : 
            ServiceResponse<ProductDTO>.FromError(new(HttpStatusCode.NotFound, "Product not found!", ErrorCodes.EntityNotFound));
    }

    public async Task<ServiceResponse<PagedResponse<ProductDTO>>> GetProducts(PaginationSearchQueryParams pagination, CancellationToken cancellationToken = default)
    {
        var result = await _repository.PageAsync(pagination, new ProductProjectionSpec(pagination.Search), cancellationToken);

        return ServiceResponse<PagedResponse<ProductDTO>>.ForSuccess(result);
    }

    public async Task<ServiceResponse> UpdateProduct(ProductUpdateDTO productUpdateDTO, UserDTO requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser.Role != UserRoleEnum.Personnel)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only personnel can update products!", ErrorCodes.CannotAdd));
        }

        var product = await _repository.GetAsync<Product>(productUpdateDTO.Id, cancellationToken);

        if (product == null)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.BadRequest, "Product not found!", ErrorCodes.EntityNotFound));
        }

        var owner = await _userService.GetUser(product.OwnerId, cancellationToken);

        if (owner.Result == null)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.BadRequest, "Owner not found!", ErrorCodes.EntityNotFound));
        }

        if (owner.Result.Id != requestingUser.Id)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "You are not the owner of this product!", ErrorCodes.NotOwner));
        }

        if (productUpdateDTO.Stock < 0)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.BadRequest, "Stock cannot be negative!", ErrorCodes.InvalidStock));
        }

        if (productUpdateDTO.Price < 0)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.BadRequest, "Price cannot be negative!", ErrorCodes.InvalidPrice));
        }

        product.Description = productUpdateDTO.Description ?? product.Description;
        product.Price = productUpdateDTO.Price ?? product.Price;
        product.Stock = productUpdateDTO.Stock ?? product.Stock;
        product.ImageUrl = productUpdateDTO.ImageUrl ?? product.ImageUrl;

        await _repository.UpdateAsync(product, cancellationToken);

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse<PagedResponse<ProductDTO>>> GetProductsByOwnerId(PaginationSearchQueryParams pagination, UserDTO requestingUser, CancellationToken cancellationToken = default)
    {
        var result = await _repository.PageAsync(pagination, new ProductProjectionSpec(requestingUser.Id, pagination.Search), cancellationToken);

        return ServiceResponse<PagedResponse<ProductDTO>>.ForSuccess(result);
    }
}
