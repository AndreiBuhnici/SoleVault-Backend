using System.Net;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;
using MobyLabWebProgramming.Core.Errors;
using MobyLabWebProgramming.Core.Requests;
using MobyLabWebProgramming.Core.Responses;
using MobyLabWebProgramming.Core.Specifications;
using MobyLabWebProgramming.Infrastructure.Database;
using MobyLabWebProgramming.Infrastructure.Repositories.Interfaces;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;

namespace MobyLabWebProgramming.Infrastructure.Services.Implementations;

public class OrderService : IOrderService
{
    private readonly IRepository<WebAppDatabaseContext> _repository;
    private readonly IOrderItemService _orderItemService;
    private readonly ICartService _cartService;
    private readonly ICartItemService _cartItemService;

    public OrderService(IRepository<WebAppDatabaseContext> repository, IOrderItemService orderItemService, ICartService cartService, ICartItemService cartItemService)
    {
        _repository = repository;
        _orderItemService = orderItemService;
        _cartService = cartService;
        _cartItemService = cartItemService;
    }

    public async Task<ServiceResponse> CreateOrder(OrderAddDTO orderAddDTO, UserDTO requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser == null)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "User not found!", ErrorCodes.EntityNotFound));
        }

        if (requestingUser.Cart == null)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.BadRequest, "Cart not found!", ErrorCodes.EntityNotFound));
        }

        if (orderAddDTO.PhoneNumber.Length != 10 || !orderAddDTO.PhoneNumber.All(char.IsDigit))
        {
            return ServiceResponse.FromError(new(HttpStatusCode.BadRequest, "Invalid phone number!", ErrorCodes.InvalidPhoneNumber));
        }

        var cart = await _repository.GetAsync<Cart>(requestingUser.Cart.Id, cancellationToken);

        if (cart == null)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.BadRequest, "Cart not found!", ErrorCodes.EntityNotFound));
        }

        var cartItems = await _cartItemService.GetCartItems(cart.Id, cancellationToken);

        if (cartItems == null || cartItems.Result == null)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.BadRequest, "Cart items not found!", ErrorCodes.EntityNotFound));
        }

        if (cartItems.Result.Count == 0)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.BadRequest, "Cart is empty!", ErrorCodes.CartEmpty));
        }

        var order = await _repository.AddAsync(new Order
        {
            UserId = requestingUser.Id,
            OrderDate = DateTime.Now,
            DeliveryDate = DateTime.Now.AddDays(new Random().Next(3, 7)).Date,
            ShippingAddress = orderAddDTO.ShippingAddress,
            PhoneNumber = orderAddDTO.PhoneNumber,
            Status = "Pending",
            Total = cartItems.Result.Sum(ci => ci.Product.Price * ci.Quantity)
        }, cancellationToken);

        foreach (var cartItem in cartItems.Result)
        {
            await _orderItemService.AddOrderItem(new OrderItemAddDTO
            {
                OrderId = order.Id,
                ProductId = cartItem.Product.Id,
                Quantity = cartItem.Quantity,
                Price = cartItem.Product.Price * cartItem.Quantity
            }, cancellationToken);
        }

        CartDTO cartDTO = new()
        {
            Id = cart.Id,
            Size = cart.Size,
            TotalPrice = cart.TotalPrice
        };

        await _cartService.ClearCart(new ClearCartDTO() { Bought = true}, cartDTO, cancellationToken);

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse<OrderDTO>> GetOrder(Guid orderId, UserDTO requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser == null)
        {
            return ServiceResponse<OrderDTO>.FromError(new(HttpStatusCode.Forbidden, "User not found!", ErrorCodes.EntityNotFound));
        }

        var order = await _repository.GetAsync<Order>(orderId, cancellationToken);

        if (order == null)
        {
            return ServiceResponse<OrderDTO>.FromError(new(HttpStatusCode.BadRequest, "Order not found!", ErrorCodes.EntityNotFound));
        }

        if (order.UserId != requestingUser.Id)
        {
            return ServiceResponse<OrderDTO>.FromError(new(HttpStatusCode.Forbidden, "User not authorized to view this order!", ErrorCodes.NotOwner));
        }

        var orderItems = await _orderItemService.GetOrderItems(orderId, cancellationToken);

        if (orderItems == null || orderItems.Result == null)
        {
            return ServiceResponse<OrderDTO>.FromError(new(HttpStatusCode.BadRequest, "Order items not found!", ErrorCodes.EntityNotFound));
        }

        var orderDTO = new OrderDTO
        {
            Id = order.Id,
            OrderItems = orderItems.Result,
            Total = order.Total,
            OrderDate = order.OrderDate,
            DeliveryDate = order.DeliveryDate,
            ShippingAddress = order.ShippingAddress,
            PhoneNumber = order.PhoneNumber,
            Status = order.Status,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt
        };

        return ServiceResponse<OrderDTO>.ForSuccess(orderDTO);
    }

    public async Task<ServiceResponse<PagedResponse<OrderDTO>>> GetOrders(PaginationSearchQueryParams pagination, UserDTO requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser == null)
        {
            return ServiceResponse<PagedResponse<OrderDTO>>.FromError(new(HttpStatusCode.Forbidden, "User not found!", ErrorCodes.EntityNotFound));
        }

        var orders = await _repository.PageAsync(pagination, new OrderProjectionSpec(requestingUser.Id, pagination.Search), cancellationToken);

        for (int i = 0; i < orders.Data.Count; i++)
        {
            if (orders.Data[i].Status == "Delivered")
            {
                continue;
            }

            orders.Data[i].Status = orders.Data[i].DeliveryDate < DateTime.Now ? "Delivered" : orders.Data[i].Status;

            var order = await _repository.GetAsync<Order>(orders.Data[i].Id, cancellationToken);

            if (order == null)
            {
                continue;
            }

            if (order.Status != orders.Data[i].Status)
            {
                order.Status = orders.Data[i].Status;
                await _repository.UpdateAsync(order, cancellationToken);
            }
        }

        return ServiceResponse<PagedResponse<OrderDTO>>.ForSuccess(orders);
    }
}