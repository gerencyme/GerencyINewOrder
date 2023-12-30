using Domain.Interfaces.IServices;
using Domain.Utils.HttpStatusExceptionCustom;
using Domain.Views;
using GerencyINewOrderApi.Views;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using System.Security.Claims;

namespace Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class NewOrderController : ControllerBase
    {
        private readonly INewOrderServices _newOrderServices;
        public NewOrderController(INewOrderServices newOrderServices)
        {
            _newOrderServices = newOrderServices;
        }

        [Produces("application/json")]
        [HttpPost("/api/GetByNewOrderId")]
        public async Task<IActionResult> GetByNewOrderId([FromBody] Guid orderId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {

                if (userId != null)
                {
                    var returnOrder = await _newOrderServices.GetByEntityId(orderId);

                    if (returnOrder.CompanieCNPJ == userId)
                    {
                        return Ok(returnOrder);
                    }

                }

                return BadRequest("Pedido não encontrado");
            }
            catch (HttpStatusExceptionCustom ex)
            {

                return StatusCode(ex.StatusCode, ex.Message);
            }

        }

        [Produces("application/json")]
        [HttpPost("/api/GetAllNewOrder")]
        public async Task<IActionResult> GetGetAllNewOrderAll()
        {
            try
            {
                var returnOrder = await _newOrderServices.ListNewOrder();
                return Ok(returnOrder);
            }
            catch (HttpStatusExceptionCustom ex)
            {

                return StatusCode(ex.StatusCode, ex.Message);
            }
        }

        [Produces("application/json")]
        [HttpPost("/api/GetLast10NewOrders")]
        public async Task<IActionResult> GetLast10NewOrders([FromBody] CnpjView cnpj)
        {
            var userCNPJ = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            try
            {

                if (userCNPJ != null)
                {

                    if (cnpj.CompanieCNPJ == userCNPJ)
                    {
                        var returnGetLast10NewOrders = await _newOrderServices.GetLast10NewOrders(cnpj.CompanieCNPJ);
                        return Ok(returnGetLast10NewOrders);
                    }

                }

                return BadRequest("usuário não encontrado");
            }
            catch (HttpStatusExceptionCustom ex)
            {

                return StatusCode(ex.StatusCode, ex.Message);
            }
        }

        [Produces("application/json")]
        [HttpPost("/api/GetOrdersByDateRangeWithPagination")]
        public async Task<IActionResult> GetOrdersByDateRangeWithPagination([FromBody] GetOrderView getOrderView)
        {
            var userCNPJ = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            try
            {

                if (userCNPJ != null)
                {

                    if (getOrderView.CompanieCNPJ == userCNPJ)
                    {
                        var returnGetLast10NewOrders = await _newOrderServices.GetOrdersByDateRangeWithPagination(getOrderView);
                        return Ok(returnGetLast10NewOrders);
                    }

                }

                return BadRequest("usuário não encontrado");
            }
            catch (HttpStatusExceptionCustom ex)
            {

                return StatusCode(ex.StatusCode, ex.Message);
            }
        }

        [Produces("application/json")]
        [HttpPost("/api/AddNewOrder")]
        public async Task<IActionResult> AddNewOrder([FromBody] NewOrderAddView register)
        {
            var userCNPJ = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            try
            {

                if (userCNPJ != null)
                {

                    if (register.CompanieCNPJ == userCNPJ)
                    {
                        var result = await _newOrderServices.AddNewOrder(register);
                        return Ok(result);
                    }

                }

                return BadRequest("usuário não encontrado");
            }
            catch (HttpStatusExceptionCustom ex)
            {

                return StatusCode(ex.StatusCode, ex.Message);
            }
        }

        [Produces("application/json")]
        [HttpPost("/api/UpdateNewOrder")]
        public async Task<IActionResult> UpdateNewOrder([FromBody] NewOrderUpdateView register)
        {
            var userCNPJ = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            try
            {
                try
                {

                    if (userCNPJ != null)
                    {

                        if (register.CompanieCNPJ == userCNPJ)
                        {
                            var result = await _newOrderServices.UpdateNewOrder(register);
                            return Ok(result);
                        }

                    }

                    return BadRequest("usuário não encontrado");
                }
                catch (HttpStatusExceptionCustom ex)
                {

                    return StatusCode(ex.StatusCode, ex.Message);
                }

            }
            catch (HttpStatusExceptionCustom ex)
            {

                return StatusCode(ex.StatusCode, ex.Message);
            }
        }

        [Produces("application/json")]
        [HttpDelete("/api/DeleteByEntityId")]
        public async Task<IActionResult> DeleteByEntityId([FromBody] Guid orderId, string cnpj)
        {
            var userCNPJ = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {

                if (userCNPJ != null)
                {

                    if (cnpj == userCNPJ)
                    {
                        // Adicione aqui a lógica para excluir a entidade com o ID fornecido
                        await _newOrderServices.DeleteNewOrder(orderId);

                        return Ok("Entidade excluída com sucesso");
                    }

                }

                return BadRequest("usuário não encontrado");
            }
            catch (HttpStatusExceptionCustom ex)
            {

                return StatusCode(ex.StatusCode, ex.Message);
            }
        }
    }
}
