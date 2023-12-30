using AutoMapper;
using Domain.Interfaces.IRepositorys;
using Domain.Interfaces.IServices;
using Domain.Utils.HttpStatusExceptionCustom;
using Domain.Views;
using Entities.Entities;
using GerencyINewOrderApi.Views;

namespace Domain.Services
{
    public class NewOrderServices : INewOrderServices
    {
        private readonly IRepositoryNewOrder _IrepositoryNewOrder;
        private readonly IMapper _mapper;
        //private readonly HttpClient _httpClient;

        public NewOrderServices(IRepositoryNewOrder IrepositoryNewOrder, IMapper mapper)
        {
            _IrepositoryNewOrder = IrepositoryNewOrder;
            _mapper = mapper;
            //_httpClient = httpClient;
        }

        public async Task<NewOrderUpdateView> AddNewOrder(NewOrderAddView objeto)
        {

            if (string.IsNullOrWhiteSpace(objeto.CompanieCNPJ))
            {
                throw new HttpStatusExceptionCustom(StatusCodeEnum.NotAcceptable, "CNPJ é obrigatório.");
            }
            if (string.IsNullOrWhiteSpace(objeto.CompanyId.ToString()))
            {
                throw new HttpStatusExceptionCustom(StatusCodeEnum.NotAcceptable, "Company Id é obrigatório.");
            }

            var newOrder = _mapper.Map<NewOrder>(objeto);

            await _IrepositoryNewOrder.Add(newOrder);

            var returnObjeto = _mapper.Map<NewOrderUpdateView>(newOrder);

            return returnObjeto;
        }

        public async Task<string> DeleteNewOrder(Guid idNewOrder)
        {
            await _IrepositoryNewOrder.DeleteNewOrder(idNewOrder);

            return "excluido com sucesso";
        }

        public async Task<NewOrder> GetByEntityId(Guid idNewOrder)
        {
            if (string.IsNullOrWhiteSpace(idNewOrder.ToString()))
            {
                throw new HttpStatusExceptionCustom(StatusCodeEnum.NotAcceptable, "Order Id é obrigatório.");
            }

            var getDemand = await _IrepositoryNewOrder.GetById(idNewOrder);

            return getDemand;
        }

        public async Task<List<NewOrder>> ListNewOrder()
        {
            var list = await _IrepositoryNewOrder.GetAll();
            return list;
        }

        public async Task<List<NewOrder>> GetLast10NewOrders(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
            {
                throw new HttpStatusExceptionCustom(StatusCodeEnum.NotAcceptable, "CNPJ é obrigatório.");
            }
            var listGetLast10NewOrders = await _IrepositoryNewOrder.GetLast10NewOrders(cnpj);

            return listGetLast10NewOrders;
        }

        public async Task<NewOrderUpdateView> UpdateNewOrder(NewOrderUpdateView objeto)
        {
            if (string.IsNullOrWhiteSpace(objeto.CompanieCNPJ))
            {
                throw new HttpStatusExceptionCustom(StatusCodeEnum.NotAcceptable, "CNPJ é obrigatório.");
            }
            if (string.IsNullOrWhiteSpace(objeto.CompanyId.ToString()))
            {
                throw new HttpStatusExceptionCustom(StatusCodeEnum.NotAcceptable, "Company Id é obrigatório.");
            }
            if (string.IsNullOrWhiteSpace(objeto.OrderId.ToString()))
            {
                throw new HttpStatusExceptionCustom(StatusCodeEnum.NotAcceptable, "Order Id é obrigatório.");
            }

            var newOrderUpdate = _mapper.Map<NewOrder>(objeto);

            await _IrepositoryNewOrder.UpdateNewOrder(newOrderUpdate, objeto.OrderId);
            

            return objeto;
        }

        public async Task<List<NewOrder>> GetOrdersByDateRangeWithPagination(GetOrderView paginatiionNeworder)
        {
           var listNewOrders = await _IrepositoryNewOrder.GetOrdersByDateRangeWithPagination(
                                                         paginatiionNeworder.CompanieCNPJ, paginatiionNeworder.StartDate, paginatiionNeworder.EndDate,
                                                         paginatiionNeworder.PageNumber, paginatiionNeworder.PageSize);

            return listNewOrders;
        }


        /*public async Task<List<NewOrder>> ListNewOrder2()
        {
            // Exemplo de uso do método Get para obter a lista de novos pedidos de um serviço web fictício
            string apiUrl = "https://api.example.com/neworders";
            HttpResponseMessage getResponse = await _httpClient.GetAsync(apiUrl);

            if (getResponse.IsSuccessStatusCode)
            {
                // Leitura e deserialização da resposta JSON
                List<NewOrder> newOrders = await getResponse.ReadContentAs<List<NewOrder>>();
                return newOrders;
            }
            else
            {
                Console.WriteLine($"Falha ao obter a lista de novos pedidos via API. Razão: {getResponse.ReasonPhrase}");
                return null;
            }
        }

        public async Task AddNewOrder2(ObjectId demandId, string observation, DateTime date)
        {
            // Criação de um novo pedido
            var newOrder = new NewOrder();
            await _IrepositoryNewOrder.Add(newOrder);

            // Exemplo de uso do método PostAsJson para enviar o novo pedido para um serviço web fictício
            string apiUrl = "https://api.example.com/neworders";
            HttpResponseMessage postResponse = await _httpClient.PostAsJson(apiUrl, newOrder);

            if (postResponse.IsSuccessStatusCode)
            {
                // O pedido foi enviado com sucesso para o serviço web
                Console.WriteLine("Novo pedido adicionado com sucesso via API.");
            }
            else
            {
                Console.WriteLine($"Falha ao adicionar novo pedido via API. Razão: {postResponse.ReasonPhrase}");
            }

            return;
        }*/

    }
}
