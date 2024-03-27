using System.Net;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;
using MobyLabWebProgramming.Core.Errors;
using MobyLabWebProgramming.Core.Responses;
using MobyLabWebProgramming.Core.Specifications;
using MobyLabWebProgramming.Infrastructure.Database;
using MobyLabWebProgramming.Infrastructure.Repositories.Interfaces;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;

namespace MobyLabWebProgramming.Infrastructure.Services.Implementations;

public class CartItemService : ICartItemService
{
    private readonly IRepository<WebAppDatabaseContext> _repository;

    public CartItemService(IRepository<WebAppDatabaseContext> repository)
    {
        _repository = repository;
    }

    public async Task<ServiceResponse> AddCartItem(CartItemAddDTO cartItemAddDTO, UserDTO requestingUser, CancellationToken cancellationToken = default)
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

        var product = await _repository.GetAsync<Product>(cartItemAddDTO.ProductId, cancellationToken);

        if (product == null)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.BadRequest, "Product not found!", ErrorCodes.EntityNotFound));
        }

        if (product.Stock < cartItemAddDTO.Quantity)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.BadRequest, "Not enough stock!", ErrorCodes.NotEnoughStock));
        }

        if (cartItemAddDTO.Quantity <= 0)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.BadRequest, "Quantity must be greater than 0!", ErrorCodes.InvalidQuantity));
        }

        var cartItem = await _repository.GetAsync(new CartItemProjectionSpec(cartItemAddDTO.ProductId, requestingUser.Cart.Id), cancellationToken);

        if (cartItem == null)
        {
            var newCartItem = new CartItem
            {
                CartId = requestingUser.Cart.Id,
                ProductId = cartItemAddDTO.ProductId,
                Quantity = cartItemAddDTO.Quantity,
                Price = product.Price * cartItemAddDTO.Quantity
            };
            await _repository.AddAsync(newCartItem, cancellationToken);

            cart.Size += cartItemAddDTO.Quantity;
            cart.TotalPrice += product.Price * cartItemAddDTO.Quantity;
            await _repository.UpdateAsync(cart, cancellationToken);
        }
        else
        {
            await UpdateCartItem(new CartItemUpdateDTO
            (
                Id: cartItem.Id,
                Quantity: cartItem.Quantity + cartItemAddDTO.Quantity
            ), requestingUser, cancellationToken);
        }

        product.Stock -= cartItemAddDTO.Quantity;

        await _repository.UpdateAsync(product, cancellationToken);

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> RemoveCartItem(CartItemRemoveDTO cartItemRemoveDTO, UserDTO requestingUser, CancellationToken cancellationToken = default)
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

        var cartItem = await _repository.GetAsync<CartItem>(cartItemRemoveDTO.Id, cancellationToken);

        if (cartItem == null)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.BadRequest, "Cart item not found!", ErrorCodes.EntityNotFound));
        }

        if (cartItem.CartId != cart.Id)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Cart item not found in the user's cart!", ErrorCodes.EntityNotFound));
        }

        await _repository.DeleteAsync<CartItem>(cartItem.Id, cancellationToken);

        cart.Size -= cartItem.Quantity;
        cart.TotalPrice -= cartItem.Price;
        await _repository.UpdateAsync(cart, cancellationToken);

        if (cartItemRemoveDTO.Bought == false)
        {
            var product = await _repository.GetAsync<Product>(cartItem.ProductId, cancellationToken);

            if (product == null)
            {
                return ServiceResponse.FromError(new(HttpStatusCode.BadRequest, "Product not found!", ErrorCodes.EntityNotFound));
            }

            product.Stock += cartItem.Quantity;

            await _repository.UpdateAsync(product, cancellationToken);
        }

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> UpdateCartItem(CartItemUpdateDTO cartItemUpdateDTO, UserDTO requestingUser, CancellationToken cancellationToken = default)
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

        var cartItem = await _repository.GetAsync<CartItem>(cartItemUpdateDTO.Id, cancellationToken);

        if (cartItem == null)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.BadRequest, "Cart item not found!", ErrorCodes.EntityNotFound));
        }

        if (cartItem.CartId != cart.Id)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Cart item not found in the user's cart!", ErrorCodes.EntityNotFound));
        }

        if (cartItemUpdateDTO.Quantity != null)
        {
            if (cartItemUpdateDTO.Quantity.Value == 0)
            {
                var cartItemRemoveDTO = new CartItemRemoveDTO
                {
                    Id = cartItem.Id,
                    Bought = false
                };

                await RemoveCartItem(cartItemRemoveDTO, requestingUser, cancellationToken);
            }
            else
            {
                var product = await _repository.GetAsync<Product>(cartItem.ProductId, cancellationToken);

                if (product == null)
                {
                    return ServiceResponse.FromError(new(HttpStatusCode.BadRequest, "Product not found!", ErrorCodes.EntityNotFound));
                }

                cart.Size += cartItemUpdateDTO.Quantity.Value - cartItem.Quantity;
                cart.TotalPrice += product.Price * cartItemUpdateDTO.Quantity.Value - cartItem.Price;

                await _repository.UpdateAsync(cart, cancellationToken);

                cartItem.Quantity = cartItemUpdateDTO.Quantity.Value;
                cartItem.Price = product.Price * cartItem.Quantity;
                await _repository.UpdateAsync(cartItem, cancellationToken);
            }
        }
        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse<ICollection<CartItemDTO>>> GetCartItems(UserDTO requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser == null)
        {
            return ServiceResponse<ICollection<CartItemDTO>>.FromError(new(HttpStatusCode.Forbidden, "User not found!", ErrorCodes.EntityNotFound));
        }

        if (requestingUser.Cart == null)
        {
            return ServiceResponse<ICollection<CartItemDTO>>.FromError(new(HttpStatusCode.BadRequest, "Cart not found!", ErrorCodes.EntityNotFound));
        }

        var cart = await _repository.GetAsync<Cart>(requestingUser.Cart.Id, cancellationToken);

        if (cart == null)
        {
            return ServiceResponse<ICollection<CartItemDTO>>.FromError(new(HttpStatusCode.BadRequest, "Cart not found!", ErrorCodes.EntityNotFound));
        }

        var cartItems = await _repository.ListAsync(new CartItemProjectionSpec(cart.Id, "Cart"), cancellationToken);

        return ServiceResponse<ICollection<CartItemDTO>>.ForSuccess(cartItems);
    }

    public async Task<ServiceResponse<ICollection<CartItemDTO>>> GetCartItemsByProductId(Guid productId, CancellationToken cancellationToken = default)
    {
        var cartItems = await _repository.ListAsync(new CartItemProjectionSpec(productId, "Product"), cancellationToken);

        return ServiceResponse<ICollection<CartItemDTO>>.ForSuccess(cartItems);
    }
}