using PinedaApp.Contracts;

namespace PinedaApp.Services
{
    public interface IBudgetService :IServiceBase
    {
        List<BudgetResponse> GetBudgets();
        BudgetResponse GetBudget(int id);
        BudgetResponse UpsertBudget(BudgetRequest request, out int newId, int? id = null);
        void DeleteBudget(int id);
    }
}
