using Microsoft.AspNetCore.Mvc;
using PinedaApp.Configurations;
using PinedaApp.Contracts;
using PinedaApp.Models.Errors;
using PinedaApp.Services;

namespace PinedaApp.Controllers
{
    public class PortfolioController : BaseApiController
    {
        private readonly IPortfolioService _portfolioService;
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
                ErrorResponse error = new ErrorResponse(ex.Message);
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
                ErrorResponse error = new ErrorResponse(ex.Message);
                return StatusCode(ex.ErrorCode, error);
            }
        }

        [HttpPost]
        public IActionResult CreatePortfolio(PortfolioRequest request)
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
                    ErrorResponse response = new ErrorResponse(validationEx.ValidationErrors.Errors);
                    return BadRequest(response);
                }
                else
                {
                    ErrorResponse response = new ErrorResponse(ex.Message);
                    return StatusCode(ex.ErrorCode, response);
                }
            }
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdatePortfolio(PortfolioRequest request, int id)
        {
            try
            {
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
                    ErrorResponse response = new ErrorResponse(validationEx.ValidationErrors.Errors);
                    return BadRequest(response);
                }
                else
                {
                    ErrorResponse response = new ErrorResponse(ex.Message);
                    return StatusCode(ex.ErrorCode, response);
                }
            }
        }

        [HttpDelete]
        public IActionResult DeletePortfolio(int id)
        {
            try
            {
                _portfolioService.DeletePortfolio(id);
                return NoContent();
            }
            catch (PinedaAppException ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                return StatusCode(ex.ErrorCode, response);
            }
        }
    }
}