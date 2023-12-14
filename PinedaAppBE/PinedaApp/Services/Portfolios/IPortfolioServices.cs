using PinedaApp.Contracts;

namespace PinedaApp.Services
{
    public interface IPortfolioService
    {
        List<PortfolioResponse> GetPortfolios();
        PortfolioResponse GetPortfolio(int id);
        PortfolioResponse UpsertPortfolio(PortfolioRequest request, int? id = null);
        void DeletePortfolio(int id);
    }
}