using PinedaApp.Contracts;

namespace PinedaApp.Services
{
    public interface IPortfolioService
    {
        Response GetPortfolios();
        Response GetPortfolio(int id);
        Response UpsertPortfolio(PortfolioRequest request, out int newId, int? id = null);
        void DeletePortfolio(int id);
    }
}