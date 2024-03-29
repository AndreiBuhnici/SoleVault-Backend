using System.Collections;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Requests;
using MobyLabWebProgramming.Core.Responses;

namespace MobyLabWebProgramming.Infrastructure.Services.Interfaces;

public interface ICartItemService
{
    public Task<ServiceResponse> AddCartItem(CartItemAddDTO cartItemAddDTO, CartDTO currentCart, CancellationToken cancellationToken = default);
    public Task<ServiceResponse> UpdateCartItem(CartItemUpdateDTO cartItemUpdateDTO, CartDTO currentCart, CancellationToken cancellationToken = default);
    public Task<ServiceResponse> RemoveCartItem(CartItemRemoveDTO cartItemRemoveDTO, CartDTO currentCart, CancellationToken cancellationToken = default);
    public Task<ServiceResponse<ICollection<CartItemDTO>>> GetCartItems(Guid cartId, CancellationToken cancellationToken = default);
    public Task<ServiceResponse<ICollection<CartItemDTO>>> GetCartItemsByProductId(Guid productId, CancellationToken cancellationToken = default);
}

