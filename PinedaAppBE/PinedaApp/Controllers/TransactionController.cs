using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PinedaApp.Configurations;
using PinedaApp.Contracts;
using PinedaApp.Models.Errors;
using PinedaApp.Services;

namespace PinedaApp.Controllers
{
    public class TransactionController(ITransactionService transactionService) : BaseApiController<ITransactionService>(transactionService)
    {
        [HttpGet]
        public IActionResult GetTransactions()
        {
            try
            {
                List<TransactionResponse> responses = _service.GetTransactions();
                return Ok(responses);
            }
            catch (PinedaAppException ex)
            {
                ErrorResponse response = new(ex.Message);
                return StatusCode(ex.ErrorCode, response);
            }
        }

        [HttpGet("{id:int}")]
        public IActionResult GetTransaction(int id)
        {
            try
            {
                TransactionResponse response = _service.GetTransaction(id);
                return Ok(response);
            }
            catch (PinedaAppException ex)
            {
                ErrorResponse response = new(ex.Message);
                return StatusCode(ex.ErrorCode, response);
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateTransaction([FromForm] TransactionRequest request)
        {
            try
            {
                TransactionResponse response = _service.UpsertTransaction(request, out newId);

                return CreatedAtAction
                (
                    actionName: nameof(GetTransaction),
                    routeValues: new { id = newId },
                    value: response
                );
            }
            catch (PinedaAppException ex)
            {
                if (ex.InnerException is ValidationException validationEx)
                {
                    ErrorResponse response = new(validationEx.ValidationErrors.Errors);
                    return BadRequest(response);
                }
                else
                {
                    ErrorResponse response = new(ex.Message);
                    return StatusCode(ex.ErrorCode, response);
                }
            }
        }

        [Authorize]
        [HttpPut("{id:int}")]
        public IActionResult UpdateTransaction(int id, [FromForm] TransactionRequest request)
        {
            try
            {
                int userId = GetUserId();
                if (userId == 0 || !CheckUserOwner(id, userId))
                {
                    ErrorResponse forbidden = new("Not Allowed to Update Data");
                    return StatusCode(403, forbidden);
                }
                TransactionResponse updatedTransaction = _service.UpsertTransaction(request, out newId, id);

                if (newId == id) return NoContent();

                return CreatedAtAction
                (
                    actionName: nameof(GetTransaction),
                    routeValues: new { id = newId },
                    value: updatedTransaction
                );
            }
            catch (PinedaAppException ex)
            {
                if (ex.InnerException is ValidationException validationEx)
                {
                    ErrorResponse response = new(validationEx.ValidationErrors.Errors);
                    return BadRequest(response);
                }
                else
                {
                    ErrorResponse response = new(ex.Message);
                    return StatusCode(ex.ErrorCode, response);
                }
            }
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        public IActionResult DeleteTransaction(int id)
        {
            try
            {
                int userId = GetUserId();
                if (userId == 0 || !CheckUserOwner(id, userId))
                {
                    ErrorResponse forbidden = new("Not Allowed to Delete Data");
                    return StatusCode(403, forbidden);
                }

                _service.DeleteTransaction(id);
                return NoContent();
            }
            catch (PinedaAppException ex)
            {
                ErrorResponse response = new(ex.Message);
                return StatusCode(ex.ErrorCode, response);
            }
        }
    }
}

