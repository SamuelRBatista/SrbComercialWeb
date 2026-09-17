using Domain.Entities;

namespace Domain.Interfaces;

public interface IStateReposiory
{
    Task<IEnumerable<State>> GetAllAsync(); // Buscar todos os estados
    Task<State?> GetByIdAsync(int id); // Buscar estado pelo Id
    Task<State?> GetByUfAsync(string uf);


}