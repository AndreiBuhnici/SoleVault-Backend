using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Responses;
using MobyLabWebProgramming.Infrastructure.Authorization;
using MobyLabWebProgramming.Infrastructure.Extensions;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;

namespace MobyLabWebProgramming.Backend.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class CartController : AuthorizedController
{
    private readonly ICartService _cartService;
    private readonly ICartItemService _cartItemService;

    public CartController(IUserService userService, ICartService cartService, ICartItemService cartItemService) : base(userService)
    {
        _cartService = cartService;
        _cartItemService = cartItemService;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<RequestResponse<CartDTO>>> GetCart()
    {
        var currentUser = await GetCurrentUser();
        return currentUser.Result != null ?
            this.FromServiceResponse(await _cartService.GetCart(currentUser.Result)) :
            this.ErrorMessageResult<CartDTO>(currentUser.Error);
    }

    [Authorize]
    [HttpDelete]
    public async Task<ActionResult<RequestResponse>> ClearCart()
    {
        var currentUser = await GetCurrentUser();
        return currentUser.Result != null ?
            this.FromServiceResponse(await _cartService.ClearCart(new ClearCartDTO() { Bought = false }, currentUser.Result)) :
            this.ErrorMessageResult(currentUser.Error);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<RequestResponse>> AddToCart([FromBody] CartItemAddDTO cartItemAddDTO)
    {
        var currentUser = await GetCurrentUser();
        return currentUser.Result != null ?
            this.FromServiceResponse(await _cartItemService.AddCartItem(cartItemAddDTO, currentUser.Result)) :
            this.ErrorMessageResult(currentUser.Error);
    }

    [Authorize]
    [HttpDelete]
    public async Task<ActionResult<RequestResponse>> RemoveFromCart([FromQuery] Guid cartItemId)
    {
        var currentUser = await GetCurrentUser();
        return currentUser.Result != null ?
            this.FromServiceResponse(await _cartItemService.RemoveCartItem(new CartItemRemoveDTO() { Id = cartItemId, Bought = false }, currentUser.Result)) :
            this.ErrorMessageResult(currentUser.Error);
    }

    [Authorize]
    [HttpPut]
    public async Task<ActionResult<RequestResponse>> UpdateCartItem([FromBody] CartItemUpdateDTO cartItemUpdateDTO)
    {
        var currentUser = await GetCurrentUser();
        return currentUser.Result != null ?
            this.FromServiceResponse(await _cartItemService.UpdateCartItem(cartItemUpdateDTO, currentUser.Result)) :
            this.ErrorMessageResult(currentUser.Error);
    }
}