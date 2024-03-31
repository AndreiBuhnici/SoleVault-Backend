using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.DataTransferObjects;

public class OrderDTO
{
    public Guid Id { get; set; }
    public ICollection<OrderItemDTO> OrderItems { get; set; } = default!;
    public DateTime OrderDate { get; set; }
    public DateTime DeliveryDate { get; set; }
    public string PhoneNumber { get; set; } = default!;
    public string ShippingAddress { get; set; } = default!;
    public string Status { get; set; } = default!;
    public float Total { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}