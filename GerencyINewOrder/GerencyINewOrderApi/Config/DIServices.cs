using Domain.Interfaces.IServices;
using Domain.Services;

namespace Domain.Utils
{
    public class DIServices
    {
        public void MapDependencies(IServiceCollection services)
        {
            // Mapeia as dependências relacionadas a NewOrder
            services.AddSingleton<INewOrderServices, NewOrderServices>();

        }
    }
}
