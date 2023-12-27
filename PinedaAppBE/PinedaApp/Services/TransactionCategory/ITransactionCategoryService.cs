using PinedaApp.Contracts;

namespace PinedaApp.Services
{
    public interface ITransactionCategoryService : IServiceBase
    {
        List<TransactionCategoryResponse> GetTransactionCategories();
        TransactionCategoryResponse GetTransactionCategory(int id);
        TransactionCategoryResponse UpsertTransactionCategory(TransactionCategoryRequest request, out int newId, int? id = null);
        void DeleteTransactionCategory(int id);
    }
}
