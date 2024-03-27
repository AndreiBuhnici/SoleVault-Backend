using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Responses;

namespace MobyLabWebProgramming.Infrastructure.Services.Interfaces;

public interface IOrderItemService
{
    public Task<ServiceResponse> AddOrderItem(OrderItemAddDTO orderItemAddDTO, CancellationToken cancellationToken = default);
    public Task<ServiceResponse<ICollection<OrderItemDTO>>> GetOrderItems(Guid orderId, CancellationToken cancellationToken = default);
    public Task<ServiceResponse<ICollection<OrderItemDTO>>> GetOrderItemsByProductId(Guid productId, CancellationToken cancellationToken = default);
}