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

    public OrderService(IRepository<WebAppDatabaseContext> repository, IOrderItemService orderItemService, ICartService cartService)
    {
        _repository = repository;
        _orderItemService = orderItemService;
        _cartService = cartService;
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

        var cart = await _repository.GetAsync<Cart>(requestingUser.Cart.Id, cancellationToken);

        if (cart == null)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.BadRequest, "Cart not found!", ErrorCodes.EntityNotFound));
        }

        var cartItems = await _repository.ListAsync(new CartItemProjectionSpec(requestingUser.Cart.Id), cancellationToken);

        if (cartItems.Count == 0)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.BadRequest, "Cart is empty!", ErrorCodes.EntityNotFound));
        }

        var order = await _repository.AddAsync(new Order
        {
            UserId = requestingUser.Id,
            OrderDate = DateTime.Now,
            DeliveryDate = DateTime.Now.AddDays(new Random().Next(3, 7)),
            ShippingAddress = orderAddDTO.ShippingAddress,
            Status = "Pending",
            Total = cartItems.Sum(ci => ci.Product.Price * ci.Quantity)
        }, cancellationToken);

        foreach (var cartItem in cartItems)
        {
            await _orderItemService.AddOrderItem(new OrderItemAddDTO
            {
                OrderId = order.Id,
                ProductId = cartItem.Product.Id,
                Quantity = cartItem.Quantity,
                Price = cartItem.Product.Price * cartItem.Quantity
            }, cancellationToken);
        }

        await _cartService.ClearCart(requestingUser, cancellationToken);

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

        var orders = await _repository.PageAsync(pagination, new OrderProjectionSpec(requestingUser.Id), cancellationToken);

        return ServiceResponse<PagedResponse<OrderDTO>>.ForSuccess(orders);
    }
}