using Domain.Interfaces;
using Infrastructure.Repository.Repositories;

namespace GerencyIProductApi.Config
{
    public class DIRepository
    {
        public void RegisterDependencies(IServiceCollection services)
        {
            // Registra as dependências
            services.AddSingleton<IRepositoryDemand, DemandRepository>();
            services.AddSingleton<IRepositoryProduct, ProductRepository>();
        }
    }
}
