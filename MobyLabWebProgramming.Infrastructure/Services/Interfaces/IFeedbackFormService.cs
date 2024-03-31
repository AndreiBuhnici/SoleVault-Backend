using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Responses;

namespace MobyLabWebProgramming.Infrastructure.Services.Interfaces;

public interface IFeedbackFormService
{
    public Task<ServiceResponse> AddFeedbackForm(FeedBackFormAddDTO feedBackFormAddDTO, UserDTO requestingUser, CancellationToken cancellationToken = default);
    public Task<ServiceResponse<FeedbackFormDTO>> GetFeedbackForm(UserDTO requestingUser, CancellationToken cancellationToken = default);
}