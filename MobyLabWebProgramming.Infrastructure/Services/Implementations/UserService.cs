using System.Net;
using MobyLabWebProgramming.Core.Constants;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;
using MobyLabWebProgramming.Core.Enums;
using MobyLabWebProgramming.Core.Errors;
using MobyLabWebProgramming.Core.Requests;
using MobyLabWebProgramming.Core.Responses;
using MobyLabWebProgramming.Core.Specifications;
using MobyLabWebProgramming.Infrastructure.Database;
using MobyLabWebProgramming.Infrastructure.Repositories.Interfaces;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;

namespace MobyLabWebProgramming.Infrastructure.Services.Implementations;

public class UserService : IUserService
{
    private readonly IRepository<WebAppDatabaseContext> _repository;
    private readonly ICartService _cartService;
    private readonly ILoginService _loginService;
    private readonly IMailService _mailService;

    /// <summary>
    /// Inject the required services through the constructor.
    /// </summary>
    public UserService(IRepository<WebAppDatabaseContext> repository, ILoginService loginService, IMailService mailService, ICartService cartService)
    {
        _repository = repository;
        _loginService = loginService;
        _mailService = mailService;
        _cartService = cartService;
    }

    public async Task<ServiceResponse<UserDTO>> GetUser(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _repository.GetAsync(new UserProjectionSpec(id), cancellationToken); // Get a user using a specification on the repository.

        return result != null ? 
            ServiceResponse<UserDTO>.ForSuccess(result) : 
            ServiceResponse<UserDTO>.FromError(CommonErrors.UserNotFound); // Pack the result or error into a ServiceResponse.
    }

    public async Task<ServiceResponse<PagedResponse<UserDTO>>> GetUsers(PaginationSearchQueryParams pagination, CancellationToken cancellationToken = default)
    {
        var result = await _repository.PageAsync(pagination, new UserProjectionSpec(pagination.Search), cancellationToken); // Use the specification and pagination API to get only some entities from the database.

        return ServiceResponse<PagedResponse<UserDTO>>.ForSuccess(result);
    }

    public async Task<ServiceResponse<LoginResponseDTO>> Login(LoginDTO login, CancellationToken cancellationToken = default)
    {
        var result = await _repository.GetAsync(new UserSpec(login.Email), cancellationToken);

        if (result == null) // Verify if the user is found in the database.
        {
            return ServiceResponse<LoginResponseDTO>.FromError(CommonErrors.UserNotFound); // Pack the proper error as the response.
        }

        if (result.Password != login.Password) // Verify if the password hash of the request is the same as the one in the database.
        {
            return ServiceResponse<LoginResponseDTO>.FromError(new(HttpStatusCode.BadRequest, "Wrong password!", ErrorCodes.WrongPassword));
        }

        var user = new UserLoginDTO
        {
            Id = result.Id,
            Email = result.Email,
            Name = result.Name,
            Role = result.Role,
            CartId = result.CartId ?? Guid.Empty,
        };

        return ServiceResponse<LoginResponseDTO>.ForSuccess(new()
        {
            User = user,
            Token = _loginService.GetToken(user, DateTime.UtcNow, new(7, 0, 0, 0)) // Get a JWT for the user issued now and that expires in 7 days.
        });
    }

    public async Task<ServiceResponse> Register(UserRegisterDTO user, CancellationToken cancellationToken = default)
    {
        var result = await _repository.GetAsync(new UserSpec(user.Email), cancellationToken);

        if (result != null)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Conflict, "The user already exists!", ErrorCodes.UserAlreadyExists));
        }

        await AddUser(new UserAddDTO
        {
            Email = user.Email,
            Name = user.Name,
            Password = user.Password,
            Role = UserRoleEnum.Client
        }, null, cancellationToken);

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse<int>> GetUserCount(CancellationToken cancellationToken = default) => 
        ServiceResponse<int>.ForSuccess(await _repository.GetCountAsync<User>(cancellationToken)); // Get the count of all user entities in the database.

    public async Task<ServiceResponse> AddUser(UserAddDTO user, UserDTO? requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser != null && requestingUser.Role != UserRoleEnum.Admin) // Verify who can add the user, you can change this however you se fit.
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only the admin can add users!", ErrorCodes.CannotAdd));
        }

        var result = await _repository.GetAsync(new UserSpec(user.Email), cancellationToken);

        if (result != null)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Conflict, "The user already exists!", ErrorCodes.UserAlreadyExists));
        }

        if (user.Role == UserRoleEnum.Client)
        {

            var cartResult = await _cartService.CreateCart(requestingUser, cancellationToken);

            if (cartResult == null)
            {
                return ServiceResponse.FromError(new(HttpStatusCode.BadRequest, "Cart creation failed!", ErrorCodes.CannotAdd));
            }

            if (cartResult.Result == null)
            {
                return ServiceResponse.FromError(new(HttpStatusCode.BadRequest, "Cart creation failed!", ErrorCodes.CannotAdd));
            }

            CartDTO cart = cartResult.Result;

            await _repository.AddAsync(new User
            {
                Email = user.Email,
                Name = user.Name,
                Role = user.Role,
                Password = user.Password,
                CartId = cart.Id
            }, cancellationToken);
        }
        else
        {
            await _repository.AddAsync(new User
            {
                Email = user.Email,
                Name = user.Name,
                Role = user.Role,
                Password = user.Password
            }, cancellationToken);
        }

        await _mailService.SendMail(user.Email, "Welcome!", MailTemplates.UserAddTemplate(user.Name), true, "My App", cancellationToken); // You can send a notification on the user email. Change the email if you want.

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> UpdateUser(UserUpdateDTO user, UserDTO? requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser != null && requestingUser.Role != UserRoleEnum.Admin && requestingUser.Id != user.Id) // Verify who can add the user, you can change this however you se fit.
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only the admin or the own user can update the user!", ErrorCodes.CannotUpdate));
        }

        var entity = await _repository.GetAsync(new UserSpec(user.Id), cancellationToken); 

        if (entity != null) // Verify if the user is not found, you cannot update an non-existing entity.
        {
            entity.Name = user.Name ?? entity.Name;
            entity.Password = user.Password ?? entity.Password;

            await _repository.UpdateAsync(entity, cancellationToken); // Update the entity and persist the changes.
        } else { 
            return ServiceResponse.FromError(new(HttpStatusCode.NotFound, "The user doesn't exist!", ErrorCodes.EntityNotFound)); // Pack the proper error as the response.
        }

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> DeleteUser(Guid id, UserDTO? requestingUser = default, CancellationToken cancellationToken = default)
    {
        if (requestingUser != null && requestingUser.Role != UserRoleEnum.Admin && requestingUser.Id != id) // Verify who can add the user, you can change this however you se fit.
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only the admin or the own user can delete the user!", ErrorCodes.CannotDelete));
        }

        var user = await _repository.GetAsync<User>(id, cancellationToken); // Get the user entity.

        if (user == null)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.NotFound, "The user doesn't exist!", ErrorCodes.EntityNotFound)); // Pack the proper error as the response.
        }

        if (user.CartId != null)
        {
            var cart = await _cartService.GetCart(user.CartId.Value, cancellationToken); // Get the cart of the user.

            if (cart == null || cart.Result == null)
            {
                return ServiceResponse.FromError(new(HttpStatusCode.BadRequest, "Cart not found!", ErrorCodes.EntityNotFound));
            }

            await _cartService.DeleteCart(cart.Result.Id, cancellationToken); // Delete the cart of the user.
        }

        await _repository.DeleteAsync<User>(id, cancellationToken); // Delete the entity.

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> AddUserFeedbackFormId(Guid feedbackFormId, UserDTO requestingUser, CancellationToken cancellationToken = default)
    {
        var user = await _repository.GetAsync<User>(requestingUser.Id, cancellationToken); // Get the user entity.

        if (user == null)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.BadRequest, "User not found!", ErrorCodes.EntityNotFound)); // Pack the proper error as the response.
        }

        user.FeedbackFormId = feedbackFormId; // Set the feedback request id to the user.

        await _repository.UpdateAsync(user, cancellationToken); // Update the user entity.

        return ServiceResponse.ForSuccess();
    }
}
