using PinedaApp.Configurations;
using PinedaApp.Contracts;

namespace PinedaApp.Services.Transaction
{
    public class TransactionService : ServiceBase, ITransactionService
    {
        public TransactionService(PinedaAppContext context) : base(context) { }

        public void DeleteTransaction(int id)
        {
            throw new NotImplementedException();
        }

        public Response GetTransaction(int id)
        {
            throw new NotImplementedException();
        }

        public Response GetTransactions()
        {
            throw new NotImplementedException();
        }

        public Response UpsertTransaction(TransactionRequest request, out int newId, int? id = null)
        {
            throw new NotImplementedException();
        }
    }
}
