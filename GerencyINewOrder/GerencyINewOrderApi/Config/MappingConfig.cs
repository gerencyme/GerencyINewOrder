using AutoMapper;
using Entities.Entities;
using GerencyINewOrderApi.Views;

namespace GerencyINewOrderApi.Config
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<NewOrder, NewOrderUpdateView>();
                config.CreateMap<NewOrderUpdateView, NewOrder>();
                config.CreateMap<NewOrder, NewOrderAddView>();
                config.CreateMap<NewOrderAddView, NewOrder>();
            });
            return mappingConfig;
        }
    }
}
