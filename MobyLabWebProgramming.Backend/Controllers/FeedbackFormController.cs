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
public class FeedbackFormController : AuthorizedController
{
    private readonly IFeedbackFormService _feedbackFormService;

    public FeedbackFormController(IUserService userService, IFeedbackFormService feedbackFormService) : base(userService, feedbackFormService : feedbackFormService)
    {
        _feedbackFormService = feedbackFormService;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<RequestResponse>> AddFeedbackForm([FromBody] FeedBackFormAddDTO feedBackFormAddDTO)
    {
        var currentUser = await GetCurrentUser();
        return currentUser.Result != null ?
            this.FromServiceResponse(await _feedbackFormService.AddFeedbackForm(feedBackFormAddDTO, currentUser.Result)) :
            this.ErrorMessageResult(currentUser.Error);
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<RequestResponse<FeedbackFormDTO>>> GetFeedbackForm()
    {
        var currentUserFeedbackForm = await GetCurrentUserFeedbackForm();
        return currentUserFeedbackForm.Result != null ?
            this.FromServiceResponse(await _feedbackFormService.GetFeedbackForm(currentUserFeedbackForm.Result.Id)) :
            this.ErrorMessageResult<FeedbackFormDTO>(currentUserFeedbackForm.Error);
    }
}