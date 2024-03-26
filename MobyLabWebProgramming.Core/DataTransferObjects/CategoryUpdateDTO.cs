namespace MobyLabWebProgramming.Core.DataTransferObjects;

public record CategoryUpdateDTO(Guid Id, string? Name = default, string? Description = default);