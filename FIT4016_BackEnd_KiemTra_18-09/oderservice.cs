using Microsoft.EntityFrameworkCore;
using OrderManagement.Data;
using OrderManagement.Models;
using System.Text.RegularExpressions;

namespace OrderManagement.Services
{
    public class OrderService
    {
        private readonly OrderManagementContext _context;

        public OrderService(OrderManagementContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public bool IsValidOrderNumberFormat(string? orderNumber)
        {
            if (string.IsNullOrWhiteSpace(orderNumber))
                return false;

            var regex = new Regex(@"^ORD-(\d{4})(\d{2})(\d{2})-(\d{4})$", RegexOptions.CultureInvariant);
            var match = regex.Match(orderNumber);

            if (!match.Success) return false;

            var year = int.Parse(match.Groups[1].Value);
            var month = int.Parse(match.Groups[2].Value);
            var day = int.Parse(match.Groups[3].Value);

            return DateTime.TryParse($"{year}-{month:D2}-{day:D2}", out _);
        }

        public async Task<bool> IsOrderNumberUniqueAsync(string orderNumber)
        {
            if (string.IsNullOrWhiteSpace(orderNumber)) return false;
            return !await _context.Orders.AnyAsync(o => o.OrderNumber == orderNumber);
        }

        // Xoá hoặc comment nếu không thực sự cần unique email
        // public async Task<bool> IsEmailUniqueAsync(string email) { ... }

        public async Task<(bool IsValid, string ErrorMessage, Product? Product)>
            ValidateProductAndStockAsync(int productId, int quantity)
        {
            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
                return (false, "Sản phẩm không tồn tại", null);

            if (quantity <= 0)
                return (false, "Số lượng phải lớn hơn 0", product);

            if (quantity > product.StockQuantity)
                return (false, $"Không đủ hàng. Còn lại: {product.StockQuantity}", product);

            return (true, string.Empty, product);
        }

        public (bool IsValid, string ErrorMessage) ValidateDates(DateTime orderDate, DateTime? deliveryDate)
        {
            // Thường nên cho phép orderDate = DateTime.Today (ngày hiện tại)
            if (orderDate.Date > DateTime.Today)
                return (false, "Ngày đặt hàng không được trong tương lai");

            if (deliveryDate.HasValue && deliveryDate.Value.Date < orderDate.Date)
                return (false, "Ngày giao hàng phải sau hoặc bằng ngày đặt hàng");

            return (true, string.Empty);
        }

        public async Task<OrderIndexViewModel> GetOrdersAsync(
            int page = 1,
            int pageSize = 10,
            string? searchString = null)
        {
            var query = _context.Orders
                .Include(o => o.Product)
                .AsNoTracking()
                .OrderByDescending(o => o.OrderDate);

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                searchString = searchString.Trim();
                query = query.Where(o =>
                    o.OrderNumber.Contains(searchString) ||
                    o.CustomerName.Contains(searchString) ||
                    o.CustomerEmail.Contains(searchString)); // có thể thêm nếu muốn
            }

            var total = await query.CountAsync();

            var orders = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new OrderIndexViewModel
            {
                Orders = orders,
                TotalRecords = total,
                CurrentPage = page,
                PageSize = pageSize,
                SearchString = searchString
            };
        }

        public async Task<IReadOnlyList<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            return await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == productId);
        }
    }
}