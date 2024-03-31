namespace MobyLabWebProgramming.Core.Entities;

public class Order : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;
    public ICollection<OrderItem> OrderItems { get; set; } = default!;
    public DateTime OrderDate { get; set; }
    public DateTime DeliveryDate { get; set; }
    public string ShippingAddress { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string Status { get; set; } = default!;
    public float Total { get; set; } = default!;
    
}