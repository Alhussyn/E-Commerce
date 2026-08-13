using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Models;

public class ProductImage
{
    public int Id { get; set; }
    public int ProductId { get; set; }

    [Required, StringLength(500)]
    public string Url { get; set; } = string.Empty;

    [StringLength(200)]
    public string? AltText { get; set; }

    public int SortOrder { get; set; }
    public Product? Product { get; set; }
}
