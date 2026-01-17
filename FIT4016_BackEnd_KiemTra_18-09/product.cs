using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderManagement.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Product name must be between 2 and 200 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "SKU is required")]
        [StringLength(50, ErrorMessage = "SKU must not exceed 50 characters")]
        public string Sku { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description must not exceed 500 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Stock quantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [StringLength(100, ErrorMessage = "Category must not exceed 100 characters")]
        public string Category { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation property
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}