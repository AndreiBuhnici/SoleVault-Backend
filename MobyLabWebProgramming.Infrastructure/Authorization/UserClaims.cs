using MobyLabWebProgramming.Core.Enums;

namespace MobyLabWebProgramming.Infrastructure.Authorization;

/// <summary>
/// This record is used to store the claims extracted from the JWT
/// </summary>
public record UserClaims(Guid Id, string? Name, string? Email, UserRoleEnum? Role, Guid? CartId, Guid? FeedbackFormId);
