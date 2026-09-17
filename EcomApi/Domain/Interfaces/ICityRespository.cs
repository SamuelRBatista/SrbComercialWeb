using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ICityRepository
    {
        Task<IEnumerable<City>> GetAllAsync(); // Buscar todas as cidades
        Task<IEnumerable<City>> GetByStateIdAsync(int stateId); // Buscar cidades por Estado
        Task<City?> GetByIdAsync(int id); // Buscar cidade pelo Id
    }
}
