using System.Net;
using Microsoft.AspNetCore.Identity;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;
using MobyLabWebProgramming.Core.Enums;
using MobyLabWebProgramming.Core.Errors;
using MobyLabWebProgramming.Core.Responses;
using MobyLabWebProgramming.Infrastructure.Database;
using MobyLabWebProgramming.Infrastructure.Repositories.Interfaces;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;

namespace MobyLabWebProgramming.Infrastructure.Services.Implementations;

public class FeedbackFormService : IFeedbackFormService
{
    private readonly IRepository<WebAppDatabaseContext> _repository;
    private readonly IUserService _userService;

    public FeedbackFormService(IRepository<WebAppDatabaseContext> repository, IUserService userService)
    {
        _repository = repository;
        _userService = userService;
    }

    public async Task<ServiceResponse> AddFeedbackForm(FeedBackFormAddDTO feedBackFormAddDTO, UserDTO requstingUser, CancellationToken cancellationToken = default)
    {
        if (requstingUser == null)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "User not found!", ErrorCodes.EntityNotFound));
        }

        if (requstingUser.Role != UserRoleEnum.Client)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "User is not a client!", ErrorCodes.UserPermission));
        }

        if (requstingUser.FeedbackForm != null)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.BadRequest, "Feedback form already exists!", ErrorCodes.CannotAdd));
        }

        var feedbackForm = new FeedbackForm()
        {
            Feedback = feedBackFormAddDTO.Feedback,
            OverallRating = feedBackFormAddDTO.OverallRating,
            DeliveryRating = feedBackFormAddDTO.DeliveryRating,
            FavoriteFeatures = feedBackFormAddDTO.FavoriteFeatures
        };

        await _repository.AddAsync(feedbackForm, cancellationToken);

        await _userService.AddUserFeedbackFormId(feedbackForm.Id, requstingUser, cancellationToken);

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse<FeedbackFormDTO>> GetFeedbackForm(UserDTO requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser.FeedbackForm == null)
        {
            return ServiceResponse<FeedbackFormDTO>.FromError(new(HttpStatusCode.BadRequest, "Feedback form not found!", ErrorCodes.EntityNotFound));
        }

        var feedbackForm = await _repository.GetAsync<FeedbackForm>(requestingUser.FeedbackForm.Id, cancellationToken);

        if (feedbackForm == null)
        {
            return ServiceResponse<FeedbackFormDTO>.FromError(new(HttpStatusCode.BadRequest, "Feedback form not found!", ErrorCodes.EntityNotFound));
        }

        var feedbackFormDTO = new FeedbackFormDTO()
        {
            Id = feedbackForm.Id,
            Feedback = feedbackForm.Feedback,
            OverallRating = feedbackForm.OverallRating,
            DeliveryRating = feedbackForm.DeliveryRating,
            FavoriteFeatures = feedbackForm.FavoriteFeatures,
            CreatedAt = feedbackForm.CreatedAt
        };

        return ServiceResponse<FeedbackFormDTO>.ForSuccess(feedbackFormDTO);
    }
}
