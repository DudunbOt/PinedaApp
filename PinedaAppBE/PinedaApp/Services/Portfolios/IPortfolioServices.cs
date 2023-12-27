using PinedaApp.Contracts;

namespace PinedaApp.Services
{
    public interface IPortfolioService : IServiceBase
    {
        List<PortfolioResponse> GetPortfolios();
        PortfolioResponse GetPortfolio(int id);
        PortfolioResponse UpsertPortfolio(PortfolioRequest request, out int newId, int? id = null);
        void DeletePortfolio(int id);
    }
}