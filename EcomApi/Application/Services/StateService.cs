using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class StateService
{
    private readonly IStateReposiory _stateRepository;

    public StateService(IStateReposiory stateRepository)
    {
        _stateRepository = stateRepository;
    }

      public async Task<IEnumerable<State>> GetAllAsync(){
         return await _stateRepository.GetAllAsync();
    }

}