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

public class OrderItemService : IOrderItemService
{
    private readonly IRepository<WebAppDatabaseContext> _repository;

    public OrderItemService(IRepository<WebAppDatabaseContext> repository)
    {
        _repository = repository;
    }

    public async Task<ServiceResponse> AddOrderItem(OrderItemAddDTO orderItemAddDTO, CancellationToken cancellationToken = default)
    {
        var orderItem = new OrderItem
        {
            OrderId = orderItemAddDTO.OrderId,
            ProductId = orderItemAddDTO.ProductId,
            Quantity = orderItemAddDTO.Quantity,
            Price = orderItemAddDTO.Price
        };

        await _repository.AddAsync(orderItem, cancellationToken);

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse<ICollection<OrderItemDTO>>> GetOrderItems(Guid orderId, CancellationToken cancellationToken = default)
    {
        var orderItems = await _repository.ListAsync(new OrderItemProjectionSpec(orderId), cancellationToken);

        return ServiceResponse<ICollection<OrderItemDTO>>.ForSuccess(orderItems);
    }
}