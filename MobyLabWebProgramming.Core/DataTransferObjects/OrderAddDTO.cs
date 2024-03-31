using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.DataTransferObjects;

public class OrderAddDTO
{
    public string PhoneNumber { get; set; } = default!;
    public string ShippingAddress { get; set; } = default!;
}
