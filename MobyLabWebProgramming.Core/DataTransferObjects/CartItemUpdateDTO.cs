namespace MobyLabWebProgramming.Core.DataTransferObjects;

public record CartItemUpdateDTO(Guid Id, int? Quantity = default);