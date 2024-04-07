using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Requests;
using MobyLabWebProgramming.Core.Responses;
using MobyLabWebProgramming.Infrastructure.Authorization;
using MobyLabWebProgramming.Infrastructure.Extensions;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;

namespace MobyLabWebProgramming.Backend.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class CartController : AuthorizedController
{
    private readonly ICartItemService _cartItemService;
    private readonly ICartService _cartService;

    public CartController(IUserService userService, ICartService cartService, ICartItemService cartItemService) : base(userService, cartService)
    {
        _cartService = cartService;
        _cartItemService = cartItemService;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<RequestResponse<CartInfoDTO>>> GetCartInfo()
    {
        var currentUserCart = await GetCurrentUserCart();
        return currentUserCart.Result != null ?
            this.FromServiceResponse(await _cartService.GetCartInfo(currentUserCart.Result.Id)) :
            this.ErrorMessageResult<CartInfoDTO>(currentUserCart.Error);
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<RequestResponse<PagedResponse<CartItemDTO>>>> GetCartItems([FromQuery] PaginationSearchQueryParams pagination)
    {
        var currentUserCart = await GetCurrentUserCart();
        return currentUserCart.Result != null ?
            this.FromServiceResponse(await _cartItemService.GetCartItemsPage(pagination, currentUserCart.Result)) :
            this.ErrorMessageResult<PagedResponse<CartItemDTO>>(currentUserCart.Error);
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<RequestResponse<CartDTO>>> GetCart()
    {
        var currentUserCart = await GetCurrentUserCart();
        return currentUserCart.Result != null ?
            this.FromServiceResponse(await _cartService.GetCart(currentUserCart.Result.Id)) :
            this.ErrorMessageResult<CartDTO>(currentUserCart.Error);
    }


    

    [Authorize]
    [HttpDelete]
    public async Task<ActionResult<RequestResponse>> ClearCart()
    {
        var currentUserCart = await GetCurrentUserCart();
        return currentUserCart.Result != null ?
            this.FromServiceResponse(await _cartService.ClearCart(new ClearCartDTO() { Bought = false }, currentUserCart.Result)) :
            this.ErrorMessageResult(currentUserCart.Error);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<RequestResponse>> AddToCart([FromBody] CartItemAddDTO cartItemAddDTO)
    {
        var currentUserCart = await GetCurrentUserCart();
        return currentUserCart.Result != null ?
            this.FromServiceResponse(await _cartItemService.AddCartItem(cartItemAddDTO, currentUserCart.Result)) :
            this.ErrorMessageResult(currentUserCart.Error);
    }

    [Authorize]
    [HttpDelete]
    public async Task<ActionResult<RequestResponse>> RemoveFromCart([FromQuery] Guid cartItemId)
    {
        var currentUserCart = await GetCurrentUserCart();
        return currentUserCart.Result != null ?
            this.FromServiceResponse(await _cartItemService.RemoveCartItem(new CartItemRemoveDTO() { Id = cartItemId, Bought = false }, currentUserCart.Result)) :
            this.ErrorMessageResult(currentUserCart.Error);
    }

    [Authorize]
    [HttpPut]
    public async Task<ActionResult<RequestResponse>> UpdateCartItem([FromBody] CartItemUpdateDTO cartItemUpdateDTO)
    {
        var currentUserCart = await GetCurrentUserCart();
        return currentUserCart.Result != null ?
            this.FromServiceResponse(await _cartItemService.UpdateCartItem(cartItemUpdateDTO, currentUserCart.Result)) :
            this.ErrorMessageResult(currentUserCart.Error);
    }
}