using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PinedaApp.Configurations;
using PinedaApp.Contracts;
using PinedaApp.Models.Errors;
using PinedaApp.Services;
using System.Security.Claims;

namespace PinedaApp.Controllers
{
    public class PortfolioController(IPortfolioService portfolioService) : BaseApiController<IPortfolioService>(portfolioService)
    {
        [HttpGet]
        public IActionResult GetPortofolios()
        {
            try
            {
                Response responses = _service.GetPortfolios();
                return Ok(responses);
            }
            catch (PinedaAppException ex)
            {
                ErrorResponse error = new(ex.Message);
                return StatusCode(ex.ErrorCode, error);
            }
        }

        [HttpGet("{id:int}")]
        public IActionResult GetPortfolio(int id)
        {
            try
            {
                Response response = _service.GetPortfolio(id);
                return Ok(response);
            }
            catch (PinedaAppException ex)
            {
                ErrorResponse error = new(ex.Message);
                return StatusCode(ex.ErrorCode, error);
            }
        }

        [Authorize]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public IActionResult CreatePortfolio([FromForm] PortfolioRequest request)
        {
            try
            {
                Response response = _service.UpsertPortfolio(request, out newId);
                return CreatedAtAction
                (
                    actionName: nameof(GetPortfolio),
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
        public IActionResult UpdatePortfolio([FromForm] PortfolioRequest request, int id)
        {
            try
            {
                int userId = GetUserId();
                if (userId == 0 || !CheckUserOwner(id, userId))
                {
                    ErrorResponse forbidden = new("Not Allowed to Delete Data");
                    return StatusCode(403, forbidden);
                }
                Response response = _service.UpsertPortfolio(request, out newId);

                if (newId == id) return NoContent();

                return CreatedAtAction
                (
                    actionName: nameof(GetPortfolio),
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
        [HttpDelete]
        public IActionResult DeletePortfolio(int id)
        {
            try
            {
                int userId = GetUserId();
                if (userId == 0 || !CheckUserOwner(id, userId))
                {
                    ErrorResponse forbidden = new("Not Allowed to Delete Data");
                    return StatusCode(403, forbidden);
                }

                _service.DeletePortfolio(id);
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