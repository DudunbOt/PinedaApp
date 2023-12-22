using PinedaApp.Contracts;

namespace PinedaApp.Services
{
    public interface ITransactionService : IServiceBase
    {
        Response GetTransactions();
        Response GetTransaction(int id);
        Response UpsertTransaction(TransactionRequest request, out int newId, int? id = null);
        void DeleteTransaction(int id);
    }
}
