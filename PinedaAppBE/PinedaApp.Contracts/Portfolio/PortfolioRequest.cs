using Microsoft.AspNetCore.Http;

namespace PinedaApp.Contracts
{
    public record PortfolioRequest
    (
        int UserId,
        string Name,
        string Description,
        IFormFile? ImageFile
    );
}
