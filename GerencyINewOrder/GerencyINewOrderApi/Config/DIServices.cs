using Domain.Interfaces.IServices;
using Domain.Services;

namespace Domain.Utils
{
    public class DIServices
    {
        public void MapDependencies(IServiceCollection services)
        {
            // Mapeia as dependências relacionadas a Demand
            services.AddSingleton<IDemandServices, DemandServices>();

            // Mapeia as dependências relacionadas a Product
            services.AddSingleton<IProductServices, ProductServices>();
        }
    }
}
