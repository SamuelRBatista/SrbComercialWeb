using Domain.Entities;

namespace Domain.Interfaces;

public interface IClientReposiory
{
        Task<IEnumerable<Client>> GetAllAsync();
        Task<Client?> GetByIdAsync(int id);
        Task<IEnumerable<Client>> GetByStateIdAsync(int stateId);
        Task<IEnumerable<Client>> GetByCityIdAsync(int cityId);
        Task<Client> AddAsync(Client client);
        Task UpdateAsync(Client client);
        Task DeleteAsync(int id);


}