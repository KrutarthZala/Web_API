namespace Product_CRUD_Web_API.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public string ProductDescription { get; set; } = null!;

    public decimal ProductPrice { get; set; }

    public string? ProductImage { get; set; }

    public string? ProductImagePath { get; set; }
    public string? ProductImageSize { get; set; }
}
