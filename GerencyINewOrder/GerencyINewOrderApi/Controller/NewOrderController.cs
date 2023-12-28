using Domain.Interfaces.IServices;
using Domain.Utils.HttpStatusExceptionCustom;
using GerencyINewOrderApi.Views;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

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

        [AllowAnonymous]
        [Produces("application/json")]
        [HttpPost("/api/GetByEntityId")]
        public async Task<IActionResult> GetByEntityId([FromBody] Guid orderId)
        {

            try
            {
                var returnOrder = await _newOrderServices.GetByEntityId(orderId);
                return Ok(returnOrder);
            }
            catch (HttpStatusExceptionCustom ex)
            {

                return StatusCode(ex.StatusCode, ex.Message);
            }
        }

        [AllowAnonymous]
        [Produces("application/json")]
        [HttpPost("/api/GetAll")]
        public async Task<IActionResult> GetAll()
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

        [AllowAnonymous]
        [Produces("application/json")]
        [HttpPost("/api/AddNewOrder")]
        public async Task<IActionResult> AddNewOrder([FromBody] NewOrderAddView register)
        {
            try
            {
                var result = await _newOrderServices.AddNewOrder(register);
                return Ok(result);
            }
            catch (HttpStatusExceptionCustom ex)
            {

                return StatusCode(ex.StatusCode, ex.Message);
            }
        }

        [AllowAnonymous]
        [Produces("application/json")]
        [HttpPost("/api/UpdateNewOrder")]
        public async Task<IActionResult> UpdateNewOrder([FromBody] NewOrderUpdateView register)
        {
            try
            {
                var result = await _newOrderServices.UpdateNewOrder(register);
                return Ok(result);
            }
            catch (HttpStatusExceptionCustom ex)
            {

                return StatusCode(ex.StatusCode, ex.Message);
            }
        }

        [AllowAnonymous]
        [Produces("application/json")]
        [HttpDelete("/api/DeleteByEntityId")]
        public async Task<IActionResult> DeleteByEntityId([FromBody] Guid orderId)
        {
            try
            {
                // Adicione aqui a lógica para excluir a entidade com o ID fornecido
                await _newOrderServices.DeleteNewOrder(orderId);

                return Ok("Entidade excluída com sucesso");
            }
            catch (Exception ex)
            {
                // Trate outras exceções conforme necessário
                return StatusCode(500, $"Erro ao excluir a entidade: {ex.Message}");
            }
        }

    }
}
