using Domain.Entities;

namespace Domain.Interfaces;

public interface ISupplierRepository
{

Task<IEnumerable<Supplier>>GetAllAsync();
Task<Supplier?> GetByIdAsync(int Id);
Task<IEnumerable<Supplier>> GetByStateIdAsync(int stateId);
Task<IEnumerable<Supplier>> GetByCityIdAsync(int cityId);
Task<Supplier> AddAsync(Supplier supplier);
Task UpdateAsync(Supplier supplier);
Task DeleteAsync(int id);

}