using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderManagement.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product is required")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Order number is required")]
        [StringLength(50, ErrorMessage = "Order number must not exceed 50 characters")]
        
        public string OrderNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Customer name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Customer name must be between 2 and 100 characters")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Customer email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        
        public string CustomerEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Order date is required")]
        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        public DateTime? DeliveryDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation property
        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }

        // Computed property for status
        public string Status
        {
            get
            {
                return DeliveryDate.HasValue ? "Delivered" : "Pending";
            }
        }
    }
}