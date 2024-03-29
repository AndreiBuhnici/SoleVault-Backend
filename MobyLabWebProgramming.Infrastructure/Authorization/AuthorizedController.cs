using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Enums;
using MobyLabWebProgramming.Core.Responses;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;

namespace MobyLabWebProgramming.Infrastructure.Authorization;

/// <summary>
/// This abstract class is used as a base class for controllers that need to get current information about the user from the database.
/// </summary>
public abstract class AuthorizedController : ControllerBase
{
    private UserClaims? _userClaims;
    protected readonly IUserService UserService;
    protected readonly ICartService? CartService;
    protected readonly IFeedbackFormService? FeedbackFormService;

    protected AuthorizedController(IUserService userService, ICartService? cartService = default, IFeedbackFormService? feedbackFormService = default) 
    {
        UserService = userService;
        if (cartService != null)
            CartService = cartService;
        if (feedbackFormService != null)
            FeedbackFormService = feedbackFormService;
    }

    /// <summary>
    /// This method extracts the claims from the JWT into an object.
    /// It also caches the object if used a second time.
    /// </summary>
    protected UserClaims ExtractClaims()
    {
        if (_userClaims != null)
        {
            return _userClaims;
        }

        var enumerable = User.Claims.ToList();
        var userId = enumerable.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(x => Guid.Parse(x.Value)).FirstOrDefault();
        var email = enumerable.Where(x => x.Type == ClaimTypes.Email).Select(x => x.Value).FirstOrDefault();
        var name = enumerable.Where(x => x.Type == ClaimTypes.Name).Select(x => x.Value).FirstOrDefault();
        var role = enumerable.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).FirstOrDefault();
        var cartId = enumerable.Where(x => x.Type == "CartId").Select(x => Guid.Parse(x.Value)).FirstOrDefault();
        var feedbackFormId = enumerable.Where(x => x.Type == "FeedbackFormId").Select(x => Guid.Parse(x.Value)).FirstOrDefault();

        UserRoleEnum userRoleEnum = role switch
        {
            "Admin" => UserRoleEnum.Admin,
            "Client" => UserRoleEnum.Client,
            "Personnel" => UserRoleEnum.Personnel,
            _ => UserRoleEnum.Client
        };

        _userClaims = new(userId, name, email, userRoleEnum, cartId, feedbackFormId);

        return _userClaims;
    }

    /// <summary>
    /// This method also gets the currently logged user information from the database to provide more information to authorization verifications.
    /// </summary>
    protected Task<ServiceResponse<UserDTO>> GetCurrentUser()
    {
        return UserService.GetUser(ExtractClaims().Id);
    }

    protected Task<ServiceResponse<CartDTO>> GetCurrentUserCart()
    {
        return CartService!.GetCart(ExtractClaims().CartId ?? Guid.Empty);
    }

    protected Task<ServiceResponse<FeedbackFormDTO>> GetCurrentUserFeedbackForm()
    {
        return FeedbackFormService!.GetFeedbackForm(ExtractClaims().FeedbackFormId ?? Guid.Empty);
    }
}
