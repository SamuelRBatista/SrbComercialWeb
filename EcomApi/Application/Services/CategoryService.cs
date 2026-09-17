using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class CategoryService
{
    private readonly ICategoryRepository _repository;

    public CategoryService(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Category>> GetAllAsync() => await _repository.GetAllAsync();
    public async Task<Category> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);
    public async Task<Category> AddAsync(Category category) => await _repository.AddAsync(category);
    public async Task UpdateAsync(Category category) => await _repository.UpdateAsync(category);
    public async Task DeleteAsync(int id) => await _repository.DeleteAsync(id);
}
