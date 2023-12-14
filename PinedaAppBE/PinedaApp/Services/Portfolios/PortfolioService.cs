using PinedaApp.Configurations;
using PinedaApp.Contracts;
using PinedaApp.Models;
using PinedaApp.Models.Errors;
using System.Security.Cryptography;

namespace PinedaApp.Services
{
    public class PortfolioService(PinedaAppContext context) : IPortfolioService
    {
        private readonly PinedaAppContext _context = context;

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

        public PortfolioResponse GetPortfolio(int id)
        {
            Portfolio portfolio = _context.Portfolio.FirstOrDefault(p => p.Id == id);
            if (portfolio == null)
            {
                throw new PinedaAppException($"Portfolio with Id: {id} not found", 404);
            }

            return CreatePortfolioResponse(portfolio);
        }

        public List<PortfolioResponse> GetPortfolios()
        {
            List<Portfolio> portfolios = _context.Portfolio.ToList();
            if (portfolios.Count == 0)
            {
                throw new PinedaAppException("No Data", 404);
            }

            List<PortfolioResponse> responses = new List<PortfolioResponse>();
            foreach (Portfolio portfolio in portfolios)
            {
                PortfolioResponse response = CreatePortfolioResponse(portfolio);
                responses.Add(response);
            }

            return responses;
        }

        public PortfolioResponse UpsertPortfolio(PortfolioRequest request, int? id = null)
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
                _context.SaveChanges();
                return CreatePortfolioResponse(portfolio);
            }

            toUpdate.Name = portfolio.Name;
            toUpdate.Description = portfolio.Description;
            toUpdate.ImageFilePath = portfolio.ImageFilePath;
            toUpdate.LastUpdatedAt = DateTime.Now;

            _context.Portfolio.Add(toUpdate);
            _context.SaveChanges();
            return CreatePortfolioResponse(portfolio);
        }

        private Portfolio? BindPortfolioFromRequest(PortfolioRequest request)
        {
            ValidationErrors checks = ValidatePortfolio(request);
            if (checks.HasErrors)
            {
                throw new PinedaAppException("Validation Error", 400, new ValidationException(checks));
            }

            string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "Asset", "Portfolio");
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            string fullFilePath = null;
            if (request.ImageFile != null)
            {
                using SHA256 sha256 = SHA256.Create();
                byte[] fileBytes = new byte[request.ImageFile.Length];
                using (Stream fs = request.ImageFile.OpenReadStream())
                {
                    fs.Read(fileBytes, 0, (int)fileBytes.Length);
                }

                byte[] hashBytes = sha256.ComputeHash(fileBytes);
                string hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty);

                string filename = hash + Path.GetExtension(request.ImageFile.FileName);
                fullFilePath = Path.Combine(uploadFolder, filename);

                if (!File.Exists(fullFilePath))
                {
                    using FileStream fileStream = new FileStream(fullFilePath, FileMode.Create);
                    request.ImageFile.CopyTo(fileStream);
                }
            }

            Portfolio portfolio = new Portfolio()
            {
                UserId = request.UserId,
                Name = request.Name,
                Description = request.Description,
                ImageFilePath = fullFilePath,
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
            if (request.ImageFile.Length > 0)
            {
                string extension = Path.GetExtension(request.ImageFile.FileName).ToLower();
                long fileSize = request.ImageFile.Length / 1024;
                if (allowedExtension.Contains(extension))
                {
                    validationErrors.AddError($"Image File type must be either {string.Join(", ", allowedExtension)}");
                }

                if (fileSize > 5)
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