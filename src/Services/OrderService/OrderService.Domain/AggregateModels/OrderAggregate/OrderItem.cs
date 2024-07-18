using OrderService.Domain.SeedWork;
using System.ComponentModel.DataAnnotations;

namespace OrderService.Domain.AggregateModels.OrderAggregate;

public class OrderItem : BaseEntity<int>, IValidatableObject
{
    public int ProductId { get; set; }

    public string ProductName { get; set; }

    public string PictureUrl { get; set; }

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    protected OrderItem()
    {
        
    }

    public OrderItem(
        int productId, string productName, string pictureUrl, 
        decimal unitPrice, int quantity)
    {
        ProductId = productId;
        ProductName = productName;
        PictureUrl = pictureUrl;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        if (Quantity <= 0)
            results.Add(new ValidationResult("Invalid number of units", new[] { "Units" }));

        return results;
    }
}
