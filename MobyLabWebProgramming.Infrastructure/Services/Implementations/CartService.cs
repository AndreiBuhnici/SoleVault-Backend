using System.Net;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;
using MobyLabWebProgramming.Core.Enums;
using MobyLabWebProgramming.Core.Errors;
using MobyLabWebProgramming.Core.Responses;
using MobyLabWebProgramming.Core.Specifications;
using MobyLabWebProgramming.Infrastructure.Database;
using MobyLabWebProgramming.Infrastructure.Repositories.Interfaces;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;

namespace MobyLabWebProgramming.Infrastructure.Services.Implementations;

public class CartService : ICartService
{
    private readonly IRepository<WebAppDatabaseContext> _repository;
    private readonly ICartItemService _cartItemService;

    public CartService(IRepository<WebAppDatabaseContext> repository, ICartItemService cartItemService)
    {
        _repository = repository;
        _cartItemService = cartItemService;
    }

    public async Task<ServiceResponse> ClearCart(UserDTO requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser == null)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "User not found!", ErrorCodes.EntityNotFound));
        }

        if (requestingUser.Cart == null)
        {
            return ServiceResponse<CartDTO>.FromError(new(HttpStatusCode.BadRequest, "Cart not found!", ErrorCodes.EntityNotFound));
        }

        var cartItems = await _cartItemService.GetCartItems(requestingUser, cancellationToken);

        if (cartItems == null || cartItems.Result == null)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.BadRequest, "Cart items not found!", ErrorCodes.EntityNotFound));
        }

        foreach (var cartItem in cartItems.Result)
        {
            await _cartItemService.RemoveCartItem(cartItem.Id, requestingUser, cancellationToken);
        }

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse<CartDTO>> CreateCart(UserDTO? requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser != null && requestingUser.Role != UserRoleEnum.Admin)
        {
            return ServiceResponse<CartDTO>.FromError(new(HttpStatusCode.Forbidden, "Only the admin can create carts!", ErrorCodes.CannotAdd));
        }

        Cart cart = new()
        {
            Size = 0,
            TotalPrice = 0
        };

        await _repository.AddAsync(cart, cancellationToken);

        CartDTO cartDTO = new()
        {
            Id = cart.Id,
            Size = cart.Size,
            TotalPrice = cart.TotalPrice
        };

        return ServiceResponse<CartDTO>.ForSuccess(cartDTO);
    }

    public async Task<ServiceResponse<CartDTO>> GetCart(UserDTO requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser == null)
        {
            return ServiceResponse<CartDTO>.FromError(new(HttpStatusCode.Forbidden, "User not found!", ErrorCodes.EntityNotFound));
        }

        if (requestingUser.Cart == null)
        {
            return ServiceResponse<CartDTO>.FromError(new(HttpStatusCode.BadRequest, "Cart not found!", ErrorCodes.EntityNotFound));
        }

        var cart = await _repository.GetAsync(new CartProjectionSpec(requestingUser.Cart.Id), cancellationToken);

        if (cart == null)
        {
            return ServiceResponse<CartDTO>.FromError(new(HttpStatusCode.BadRequest, "Cart not found!", ErrorCodes.EntityNotFound));
        }

        return ServiceResponse<CartDTO>.ForSuccess(cart);
    }

    public async Task<ServiceResponse> DeleteCart(UserDTO requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser == null)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "User not found!", ErrorCodes.EntityNotFound));
        }

        if (requestingUser.Cart == null)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.BadRequest, "Cart not found!", ErrorCodes.EntityNotFound));
        }

        var cart = await _repository.GetAsync<Cart>(requestingUser.Cart.Id, cancellationToken);

        if (cart == null)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.BadRequest, "Cart not found!", ErrorCodes.EntityNotFound));
        }

        await ClearCart(requestingUser, cancellationToken);

        await _repository.DeleteAsync<Cart>(cart.Id, cancellationToken);

        return ServiceResponse.ForSuccess();
    }
}