using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Responses;

namespace MobyLabWebProgramming.Infrastructure.Services.Interfaces;

public interface ICartService
{
    public Task<ServiceResponse<CartDTO>> CreateCart(UserDTO? requestingUser = null, CancellationToken cancellationToken = default);
    public Task<ServiceResponse<CartDTO>> GetCart(Guid id, CancellationToken cancellationToken = default);
    public Task<ServiceResponse> ClearCart(ClearCartDTO clearCartDTO, CartDTO currentCart, CancellationToken cancellationToken = default);
    public Task<ServiceResponse> DeleteCart(Guid id, CancellationToken cancellationToken = default);
}