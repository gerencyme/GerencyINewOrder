using AutoMapper;
using Domain.Interfaces.IRepositorys;
using Domain.Interfaces.IServices;
using Domain.Utils.HttpStatusExceptionCustom;
using Domain.Views;
using Entities.Entities;
using Entities.Enums;
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

            newOrder.OrderStatus = "underAnalysis";

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

        public async Task<List<OrderCardView>> GetLast12NewOrders(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
            {
                throw new HttpStatusExceptionCustom(StatusCodeEnum.NotAcceptable, "CNPJ é obrigatório.");
            }
            var listGetLast10NewOrders = await _IrepositoryNewOrder.GetLast12NewOrders(cnpj);

            var listGetLast10OrderCardView = _mapper.Map<List<OrderCardView>>(listGetLast10NewOrders);

            return listGetLast10OrderCardView;
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

        public async Task<string> UpdateIsLikedField(string cnpj, string orderId, bool isLiked)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
            {
                throw new HttpStatusExceptionCustom(StatusCodeEnum.NotAcceptable, "CNPJ é obrigatório.");
            }
            if (string.IsNullOrWhiteSpace(orderId))
            {
                throw new HttpStatusExceptionCustom(StatusCodeEnum.NotAcceptable, "Order Id é obrigatório.");
            }

            return await _IrepositoryNewOrder.UpdateIsLikedField(cnpj, orderId, isLiked);
        }

        public async Task<List<NewOrder>> GetOrdersByDateRangeWithPagination(GetOrderView paginatiionNeworder)
        {
           var listNewOrders = await _IrepositoryNewOrder.GetOrdersByDateRangeWithPagination(
                                                         paginatiionNeworder.CompanieCNPJ, paginatiionNeworder.StartDate, paginatiionNeworder.EndDate,
                                                         paginatiionNeworder.PageNumber, paginatiionNeworder.PageSize);

            return listNewOrders;
        }


        public async Task<List<OrderCardView>> GetOrdersByDateWithPagination(GetOrderView paginatiionNeworder)
        {
            var listOrders = await _IrepositoryNewOrder.GetOrdersByDateWithPagination(
                                                          paginatiionNeworder.CompanieCNPJ, paginatiionNeworder.PageNumber, paginatiionNeworder.PageSize);

            var listGetOrderCardView = _mapper.Map<List<OrderCardView>>(listOrders);

            return listGetOrderCardView;
        }

        public async Task<List<ProductIsByLikedView>> GetOrdersByIsLiked(string cnpj)
        {
            const bool isLiked = true;
            const string underAnalysis = "underAnalysis";
            const string done = "done";
            const string cancel = "canceled";
            if (string.IsNullOrWhiteSpace(cnpj))
            {
                throw new ArgumentException($"'{nameof(cnpj)}' cannot be null or whitespace.", nameof(cnpj));
            }

            var listOrders = await _IrepositoryNewOrder.GetOrdersByIsLiked(cnpj, isLiked);

            int countStatusUnderAnalise = 0;
            int countStatusAnalise = 0;
            int countStatusCnacel = 0;

            
            List<ProductIsByLikedView> listProductsByLiked2 = new List<ProductIsByLikedView>();

            foreach (var order in listOrders)
            {
                string nameProduct = order.Product.ProductName;

                // Verifica se o produto já está na lista
                ProductIsByLikedView productIsByLiked = listProductsByLiked2.FirstOrDefault(x => x.ProductName == nameProduct);

                if (productIsByLiked == null)
                {
                    // Cria um novo objeto ProductIsByLiked
                    productIsByLiked = new ProductIsByLikedView
                    {
                        
                        OrderId = order.OrderId,
                        CompanyId = order.CompanyId,
                        OrderColorIdentity = order.OrderColorIdentity,
                        ProductName = nameProduct,
                        ProductBrand = order.Product.ProductBrand,
                        ProductType = order.Product.ProductType,
                        CountStatusUnderAnalise = 0,
                        CountStatusAnalise = 0,
                        CountStatusCnacel = 0
                    };

                    listProductsByLiked2.Add(productIsByLiked);
                }

                // Atualiza o contador do status apropriado
                switch (order.OrderStatus)
                {
                    case underAnalysis:
                        productIsByLiked.CountStatusUnderAnalise++;
                        break;
                    case done:
                        productIsByLiked.CountStatusAnalise++;
                        break;
                    case cancel:
                        productIsByLiked.CountStatusCnacel++;
                        break;
                }
            }

            return listProductsByLiked2;
        }
    }
}


/*List<ProductIsByLikedView> listProductsByLiked = listOrders.GroupBy(x => x.Product.ProductName)
                                                .Select(x => new ProductIsByLikedView
                                                {
                                                    ProductName = x.Key,
                                                    CountStatusUnderAnalise = x.Where(y => y.OrderStatus == underAnalysis).Count(),
                                                    CountStatusAnalise = x.Where(y => y.OrderStatus == done).Count(),
                                                    CountStatusCnacel = x.Where(y => y.OrderStatus == cancel).Count()
                                                })
                                                .ToList();*/



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