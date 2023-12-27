using AutoMapper;
using AutoMapper.QueryableExtensions;
using PinedaApp.Configurations;
using PinedaApp.Contracts;
using PinedaApp.Models;
using PinedaApp.Models.DTO;
using PinedaApp.Models.Errors;

namespace PinedaApp.Services
{
    public class BudgetService(PinedaAppContext context, IMapper mapper) : ServiceBase(context, mapper), IBudgetService
    {
        public void DeleteBudget(int id)
        {
            Budget budget = _context.Budget.FirstOrDefault(b => b.Id == id) ?? throw new PinedaAppException($"Budget with id {id} not found", 404);
            List<Transaction> transactions = _context.Transaction.Where(t => t.BudgetId == id).ToList();

            foreach (Transaction transaction in transactions)
            {
                _context.Transaction.Remove(transaction);
            }

            _context.Budget.Remove(budget);
            _context.SaveChanges();
        }

        public BudgetResponse GetBudget(int id)
        {
            Budget budget = _context.Budget.FirstOrDefault(b => b.Id == id) ?? throw new PinedaAppException($"Budget with id {id} not found", 404);

            return CreateBudgetResponse(budget);
        }

        public List<BudgetResponse> GetBudgets()
        {
            List<Budget> budgets = _context.Budget.ToList();
            if(budgets == null || budgets.Count == 0)
            {
                throw new PinedaAppException("No Data", 404);
            }

            List<BudgetResponse> responses = [];
            foreach(Budget budget in budgets)
            {
                BudgetResponse response = CreateBudgetResponse(budget);
                responses.Add(response);
            }

            return responses;
        }

        public BudgetResponse UpsertBudget(BudgetRequest request, out int newId, int? id = null)
        {
            Budget budget = BindBudgetFromRequest(request);
            Budget toUpdate = null;
            if (id != null) toUpdate = _context.Budget.FirstOrDefault(b => b.Id == id);

            if(toUpdate == null)
            {
                _context.Budget.Add(budget);
            }
            else
            {
                toUpdate.Name = budget.Name;
                toUpdate.Goal = budget.Goal;
                toUpdate.Current = budget.Current;
                toUpdate.LastUpdatedAt = DateTime.Now;

                _context.Budget.Update(toUpdate);

                budget = toUpdate;
            }

            _context.SaveChanges();
            newId = budget.Id;

            return CreateBudgetResponse(budget);
        }

        private Budget? BindBudgetFromRequest(BudgetRequest request)
        {
            ValidationErrors checks = ValidateBudget(request);
            if (checks.HasErrors)
            {
                throw new PinedaAppException("Validation Error", 400, new ValidationException(checks));
            }

            Budget budget = new Budget()
            {
                UserId = request.UserId,
                Name = request.Name,
                Goal = request.Goal,
                Current = request.Current,
                CreatedAt = DateTime.Now,
                LastUpdatedAt = DateTime.Now
            };

            return budget;
        }

        private ValidationErrors ValidateBudget(BudgetRequest request)
        {
            ValidationErrors validationErrors = new();
            if (request == null)
            {
                validationErrors.AddError("The request is empty");
            }
            if (request.UserId == 0 || request.UserId == null)
            {
                validationErrors.AddError("User ID is empty");
            }
            if (String.IsNullOrEmpty(request.Name))
            {
                validationErrors.AddError("Budget Name is empty");
            }
            if (request.Goal <= 0)
            {
                validationErrors.AddError("Goal can't be less or equal 0");
            }
            if (request.Current < 0)
            {
                validationErrors.AddError("Current can't be less than 0");
            }

            return validationErrors;
        }

        private BudgetResponse CreateBudgetResponse(Budget budget)
        {
            IEnumerable<TransactionDto> transactions = _context.Transaction
            .Where(a => a.UserId == budget.UserId && a.BudgetId == budget.Id)
            .ProjectTo<TransactionDto>(_mapper.ConfigurationProvider);

            BudgetResponse response = new
            (
                budget.Id,
                budget.Name,
                budget.Goal,
                budget.Current,
                budget.Remaining,
                budget.CreatedAt,
                budget.LastUpdatedAt,
                transactions.Cast<object>().ToList()
            );

            return response;
        }
    }
}
