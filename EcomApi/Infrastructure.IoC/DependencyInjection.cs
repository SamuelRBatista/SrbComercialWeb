using InfraData.Context;
using InfraData.Repositories;
using Application.Interfaces;
using Application.Services;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.IoC
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            // Registrar repositórios com Dapper
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IStateReposiory, StateRepository>(); 
            services.AddScoped<ICityRepository, CityRepository>();
            services.AddScoped<IClientReposiory, ClientRepository>();
            services.AddScoped<ISupplierRepository, SupplierRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuthService, AuthService>();
           
            
            // Registrar services
           
            services.AddScoped<CategoryService>();
            services.AddScoped<ProductService>();
            services.AddScoped<StateService>();
            services.AddScoped<CityService>();
            services.AddScoped<ClientService>();
            services.AddScoped<SupplierService>();
            
            return services;
        }
    }
}