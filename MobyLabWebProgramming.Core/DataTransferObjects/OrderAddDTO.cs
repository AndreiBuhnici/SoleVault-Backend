using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.DataTransferObjects;

public class OrderAddDTO
{
    public string ShippingAddress { get; set; } = default!;
}
