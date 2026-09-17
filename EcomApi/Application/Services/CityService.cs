using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class CityService 
{

    private readonly ICityRepository _cityRepository;

    public CityService(ICityRepository cityRepository)
    {
        _cityRepository = cityRepository;
        
    }

    public async Task<IEnumerable<City>> GetAllAsync(){
         return await _cityRepository.GetAllAsync();
    }

    



}
