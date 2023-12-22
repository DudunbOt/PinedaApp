using PinedaApp.Configurations;
using PinedaApp.Contracts;
using PinedaApp.Models;
using PinedaApp.Models.Errors;

namespace PinedaApp.Services
{
    public class PortfolioService(PinedaAppContext context) : ServiceBase(context), IPortfolioService
    {
        public void DeletePortfolio(int id)
        {
            Portfolio portfolio = _context.Portfolio.FirstOrDefault(p => p.Id == id);
            if (portfolio == null)
            {
                throw new PinedaAppException($"Portfolio with id: {id} not found");
            }

            _context.Portfolio.Remove(portfolio);
            _context.SaveChanges();
        }

        public Response GetPortfolio(int id)
        {
            Portfolio portfolio = _context.Portfolio.FirstOrDefault(p => p.Id == id);
            if (portfolio == null)
            {
                throw new PinedaAppException($"Portfolio with Id: {id} not found", 404);
            }

            PortfolioResponse portfolioResponse = CreatePortfolioResponse(portfolio);
            return CreateResponse("success", ("portfolio", portfolioResponse));
        }

        public Response GetPortfolios()
        {
            List<Portfolio> portfolios = _context.Portfolio.ToList();
            if (portfolios == null || portfolios.Count == 0)
            {
                throw new PinedaAppException("No Data", 404);
            }

            List<PortfolioResponse> responses = new List<PortfolioResponse>();
            foreach (Portfolio portfolio in portfolios)
            {
                PortfolioResponse response = CreatePortfolioResponse(portfolio);
                responses.Add(response);
            }

            return CreateResponse("success", ("portfolio", responses));
        }

        public Response UpsertPortfolio(PortfolioRequest request, out int newId, int? id = null)
        {
            if (request == null) throw new PinedaAppException("No Request is Made", 400);
            Portfolio portfolio = BindPortfolioFromRequest(request);
            Portfolio toUpdate = null;

            if (id != null)
            {
                toUpdate = _context.Portfolio.FirstOrDefault(p => p.Id == id);
            }

            if (id == null || toUpdate == null)
            {
                _context.Portfolio.Add(portfolio);
            }
            else
            {
                toUpdate.Name = portfolio.Name;
                toUpdate.Description = portfolio.Description;
                if (!string.IsNullOrEmpty(toUpdate.ImageFilePath) && portfolio.ImageFilePath != null && toUpdate.ImageFilePath != portfolio.ImageFilePath)
                {
                    toUpdate.ImageFilePath = portfolio.ImageFilePath;
                }
                toUpdate.LastUpdatedAt = DateTime.Now;

                _context.Portfolio.Add(toUpdate);
                portfolio = toUpdate;
            }
           
            _context.SaveChanges();
            
            newId = portfolio.Id;

            PortfolioResponse portfolioResponse = CreatePortfolioResponse(portfolio);
            return CreateResponse("success", ("portfolio", portfolioResponse));
        }

        private Portfolio? BindPortfolioFromRequest(PortfolioRequest request)
        {
            ValidationErrors checks = ValidatePortfolio(request);
            if (checks.HasErrors)
            {
                throw new PinedaAppException("Validation Error", 400, new ValidationException(checks));
            }

            string filename = UploadMediaFile(request.ImageFile, "Assets", "Portfolio");

            Portfolio portfolio = new Portfolio()
            {
                UserId = request.UserId,
                Name = request.Name,
                Description = request.Description,
                ImageFilePath = filename,
                CreatedAt = DateTime.Now,
                LastUpdatedAt = DateTime.Now
            };

            return portfolio;
        }

        private ValidationErrors ValidatePortfolio(PortfolioRequest request)
        {
            List<string> allowedExtension = new() { ".jpg", ".png", ".jpeg" };

            ValidationErrors validationErrors = new();
            if (request == null)
            {
                validationErrors.AddError("The request is empty");
            }
            if (request.UserId == 0)
            {
                validationErrors.AddError("User Id is Required");
            }
            if (request.UserId > 0 && !_context.Users.Any(u => u.Id == request.UserId))
            {
                validationErrors.AddError($"User with Id: {request.UserId} Not Found");
            }
            if (String.IsNullOrEmpty(request.Name))
            {
                validationErrors.AddError("Portfolio Name is empty");
            }
            if (request.ImageFile != null && request.ImageFile.Length > 0)
            {
                string extension = Path.GetExtension(request.ImageFile.FileName).ToLower();
                long fileSize = request.ImageFile.Length / 1024;
                if (!allowedExtension.Contains(extension))
                {
                    validationErrors.AddError($"Image File type must be either {string.Join(", ", allowedExtension)}");
                }

                if (fileSize > 5000)
                {
                    validationErrors.AddError($"Image size must be less than 5MB");
                }
            }

            return validationErrors;
        }

        private PortfolioResponse CreatePortfolioResponse(Portfolio portfolio)
        {
            PortfolioResponse response = new PortfolioResponse
            (
                portfolio.Id,
                portfolio.UserId,
                portfolio.Name,
                portfolio.Description,
                portfolio.ImageFilePath,
                portfolio.CreatedAt,
                portfolio.LastUpdatedAt
            );

            return response;
        }


    }
}