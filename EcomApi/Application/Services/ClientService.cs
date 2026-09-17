using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class ClientService
{
    private readonly IClientReposiory _clientRepository;

    public ClientService(IClientReposiory clientRepository)  // ← Removeu o ClientValidator
    {
        _clientRepository = clientRepository;
    }

    public async Task<IEnumerable<Client>> GetAllAsync()
    {
        return await _clientRepository.GetAllAsync();
    }

    public async Task<Client?> GetByIdAsync(int id)
    {
        return await _clientRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Client>> GetByCityIdAsync(int cityId)
    {
        return await _clientRepository.GetByCityIdAsync(cityId);
    }

    public async Task<IEnumerable<Client>> GetByStateIdAsync(int stateId)
    {
        return await _clientRepository.GetByStateIdAsync(stateId);
    }

    public async Task<Client> AddAsync(Client client)
    {
        // Validação básica (opcional)
        if (string.IsNullOrWhiteSpace(client.Name))
            throw new InvalidOperationException("Nome do cliente é obrigatório");
        
        if (string.IsNullOrWhiteSpace(client.Cpf) || client.Cpf.Length != 14)
            throw new InvalidOperationException("CPF inválido");
        
        return await _clientRepository.AddAsync(client);
    }

    public async Task UpdateAsync(Client client)
    {
        await _clientRepository.UpdateAsync(client);
    }

    public async Task DeleteAsync(int id)
    {
        await _clientRepository.DeleteAsync(id);
    }
}
