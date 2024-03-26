using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Requests;
using MobyLabWebProgramming.Core.Responses;

namespace MobyLabWebProgramming.Infrastructure.Services.Interfaces;

public interface IOrderService
{
    public Task<ServiceResponse> CreateOrder(OrderAddDTO orderAddDTO, UserDTO requestingUser, CancellationToken cancellationToken = default);

    public Task<ServiceResponse<OrderDTO>> GetOrder(Guid orderId, UserDTO requestingUser, CancellationToken cancellationToken = default);

    public Task<ServiceResponse<PagedResponse<OrderDTO>>> GetOrders(PaginationSearchQueryParams pagination, UserDTO requestingUser, CancellationToken cancellationToken = default);
}