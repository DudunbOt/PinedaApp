using PinedaApp.Contracts;

namespace PinedaApp.Services
{
    public interface IPortfolioService : IServiceBase
    {
        Response GetPortfolios();
        Response GetPortfolio(int id);
        Response UpsertPortfolio(PortfolioRequest request, out int newId, int? id = null);
        void DeletePortfolio(int id);
    }
}