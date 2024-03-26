using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.DataTransferObjects;

public record ProductUpdateDTO(Guid Id, string? Description = default, float? Price = default, int? Stock = default, string? ImageUrl = default);