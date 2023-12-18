using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PinedaApp.Configurations;
using PinedaApp.Contracts;
using PinedaApp.Models.Errors;
using PinedaApp.Services;
using System.Security.Claims;

namespace PinedaApp.Controllers
{
    public class PortfolioController : BaseApiController
    {
        private readonly IPortfolioService _portfolioService;
        private readonly int _userId = 0;
        public PortfolioController(IPortfolioService portfolioService)
        {
            _portfolioService = portfolioService;
        }

        [HttpGet]
        public IActionResult GetPortofolios()
        {
            try
            {
                List<PortfolioResponse> responses = _portfolioService.GetPortfolios();
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
                PortfolioResponse response = _portfolioService.GetPortfolio(id);
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
                PortfolioResponse response = _portfolioService.UpsertPortfolio(request);
                return CreatedAtAction
                (
                    actionName: nameof(GetPortfolio),
                    routeValues: new { id = response.Id },
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
                PortfolioResponse response = _portfolioService.UpsertPortfolio(request, id);

                if (response.Id == id) return NoContent();

                return CreatedAtAction
                (
                    actionName: nameof(GetPortfolio),
                    routeValues: new { id = response.Id },
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

                _portfolioService.DeletePortfolio(id);
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