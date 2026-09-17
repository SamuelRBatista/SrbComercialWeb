using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class ProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _productRepository.GetAllAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _productRepository.GetByIdAsync(id);
    }

    public async Task<Product> AddAsync(Product product)
    {
        return await _productRepository.AddAsync(product);
    }

    public async Task UpdateAsync(Product product)
    {
        await _productRepository.UpdateAsync(product);
    }

    public async Task SellAsync(int productId, int quantity, string? reason = null, string? documentNumber = null, int? userId = null)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
            throw new KeyNotFoundException("Produto não encontrado.");

        var movement = product.Sell(quantity, reason, documentNumber, userId);

        await _productRepository.UpdateAsync(product);
        await _productRepository.AddStockMovementAsync(movement);
    }

    public async Task<IEnumerable<Domain.Entities.StockMovement>> GetStockMovementsAsync(int productId, int? limit = 50)
    {
        return await _productRepository.GetStockMovementsAsync(productId, limit);
    }

    public async Task DeleteAsync(int id)
    {
        await _productRepository.DeleteAsync(id);
    }
}