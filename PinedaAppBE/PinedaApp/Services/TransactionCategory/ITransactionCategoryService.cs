using PinedaApp.Contracts;

namespace PinedaApp.Services
{
    public interface ITransactionCategoryService : IServiceBase
    {
        Response GetTransactionCategories();
        Response GetTransactionCategory(int id);
        Response UpsertTransactionCategory(TransactionCategoryRequest request, out int newId, int? id = null);
        void DeleteTransactionCategory(int id);
    }
}
