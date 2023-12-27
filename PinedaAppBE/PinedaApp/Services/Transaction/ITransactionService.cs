using PinedaApp.Contracts;

namespace PinedaApp.Services
{
    public interface ITransactionService : IServiceBase
    {
        List<TransactionResponse> GetTransactions();
        TransactionResponse GetTransaction(int id);
        TransactionResponse UpsertTransaction(TransactionRequest request, out int newId, int? id = null);
        void DeleteTransaction(int id);
    }
}
