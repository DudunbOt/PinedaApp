using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PinedaApp.Configurations;
using PinedaApp.Contracts;
using PinedaApp.Models.Errors;
using PinedaApp.Services;

namespace PinedaApp.Controllers
{
    public class TransactionCategoryController(ITransactionCategoryService transactionCategoryService) : BaseApiController<ITransactionCategoryService>(transactionCategoryService)
    {
        [HttpGet]
        public IActionResult GetTransactionCategories()
        {
            try
            {
                Response responses = _service.GetTransactionCategories();
                return Ok(responses);
            }
            catch (PinedaAppException ex)
            {
                ErrorResponse response = new(ex.Message);
                return StatusCode(ex.ErrorCode, response);
            }
        }

        [HttpGet("{id:int}")]
        public IActionResult GetTransactionCategory(int id)
        {
            try
            {
                Response response = _service.GetTransactionCategory(id);
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
        public IActionResult CreateTransactionCategory([FromForm] TransactionCategoryRequest request)
        {
            try
            {
                Response response = _service.UpsertTransactionCategory(request, out newId);

                return CreatedAtAction
                (
                    actionName: nameof(GetTransactionCategory),
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
        public IActionResult UpdateTransactionCategory(int id, [FromForm] TransactionCategoryRequest request)
        {
            try
            {
                int userId = GetUserId();
                if (userId == 0 || !CheckUserOwner(id, userId))
                {
                    ErrorResponse forbidden = new("Not Allowed to Update Data");
                    return StatusCode(403, forbidden);
                }
                Response updatedTransactionCategory = _service.UpsertTransactionCategory(request, out newId, id);

                if (newId == id) return NoContent();

                return CreatedAtAction
                (
                    actionName: nameof(GetTransactionCategory),
                    routeValues: new { id = newId },
                    value: updatedTransactionCategory
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
        public IActionResult DeleteTransactionCategory(int id)
        {
            try
            {
                int userId = GetUserId();
                if (userId == 0 || !CheckUserOwner(id, userId))
                {
                    ErrorResponse forbidden = new("Not Allowed to Delete Data");
                    return StatusCode(403, forbidden);
                }

                _service.DeleteTransactionCategory(id);
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
