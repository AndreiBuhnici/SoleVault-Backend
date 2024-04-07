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

    public async Task<ServiceResponse> ClearCart(ClearCartDTO clearCartDTO, CartDTO currentCart, CancellationToken cancellationToken = default)
    {
        var cartItems = await _cartItemService.GetCartItems(currentCart.Id, cancellationToken);

        if (cartItems == null || cartItems.Result == null)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.BadRequest, "Cart items not found!", ErrorCodes.EntityNotFound));
        }

        foreach (var cartItem in cartItems.Result)
        {
            var cartItemRemoveDTO = new CartItemRemoveDTO()
            {
                Id = cartItem.Id,
                Bought = false
            };

            if (clearCartDTO.Bought)
            {
                cartItemRemoveDTO.Bought = true;
            }

            await _cartItemService.RemoveCartItem(cartItemRemoveDTO, currentCart, cancellationToken);
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

    public async Task<ServiceResponse<CartDTO>> GetCart(Guid id, CancellationToken cancellationToken = default)
    {
        var cart = await _repository.GetAsync(new CartProjectionSpec(id), cancellationToken);

        if (cart == null)
        {
            return ServiceResponse<CartDTO>.FromError(new(HttpStatusCode.BadRequest, "Cart not found!", ErrorCodes.EntityNotFound));
        }

        return ServiceResponse<CartDTO>.ForSuccess(cart);
    }

    public async Task<ServiceResponse<CartInfoDTO>> GetCartInfo(Guid id, CancellationToken cancellationToken = default)
    {
        var cart = await _repository.GetAsync(new CartProjectionSpec(id), cancellationToken);

        if (cart == null)
        {
            return ServiceResponse<CartInfoDTO>.FromError(new(HttpStatusCode.BadRequest, "Cart not found!", ErrorCodes.EntityNotFound));
        }

        CartInfoDTO cartInfoDTO = new()
        {
            Id = cart.Id,
            Size = cart.Size,
            TotalPrice = cart.TotalPrice,
            CreatedAt = cart.CreatedAt,
            UpdatedAt = cart.UpdatedAt
        };

        return ServiceResponse<CartInfoDTO>.ForSuccess(cartInfoDTO);
    }

    public async Task<ServiceResponse> DeleteCart(Guid id, CancellationToken cancellationToken = default)
    {
        var cart = await _repository.GetAsync<Cart>(id, cancellationToken);

        if (cart == null)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.BadRequest, "Cart not found!", ErrorCodes.EntityNotFound));
        }

        await _repository.DeleteAsync<Cart>(cart.Id, cancellationToken);

        return ServiceResponse.ForSuccess();
    }
}