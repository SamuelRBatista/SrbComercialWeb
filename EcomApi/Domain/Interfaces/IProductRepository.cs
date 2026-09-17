using Domain.Entities;

namespace Domain.Interfaces;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<Product?> GetBySkuAsync(string sku);
    Task<Product> AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(int id);
    Task<bool> IsCodeBarUniqueAsync(string barCode);
    Task<bool> IsSkuUniqueAsync(string sku, int? excludeProductId = null);
    Task<IEnumerable<Product>> SearchAsync(string? term, int? categoryId, decimal? minPrice, decimal? maxPrice, int page = 1, int pageSize = 20);
    Task AddStockMovementAsync(StockMovement movement);
    Task<IEnumerable<StockMovement>> GetStockMovementsAsync(int productId, int? limit = 50);
}