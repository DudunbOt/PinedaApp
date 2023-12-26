using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PinedaApp.Configurations;
using PinedaApp.Contracts;
using PinedaApp.Models;
using PinedaApp.Models.Errors;

namespace PinedaApp.Services
{
    public class TransactionService(PinedaAppContext context) : ServiceBase(context), ITransactionService
    {
        public void DeleteTransaction(int id)
        {
            Transaction transaction = _context.Transaction.FirstOrDefault(x => x.Id == id) ?? throw new PinedaAppException($"Transaction with id {id} not found");

            _context.Transaction.Remove(transaction);
            _context.SaveChanges();
        }

        public Response GetTransaction(int id)
        {
            Transaction transaction = _context.Transaction.FirstOrDefault(x => x.Id == id) ?? throw new PinedaAppException($"Transaction with id {id} not found");

            TransactionResponse response = CreateTransactionResponse(transaction);
            return CreateResponse("success", ("transaction", response));
        }

        public Response GetTransactions()
        {
            List<Transaction> transactions = _context.Transaction.ToList();
            if(transactions == null || transactions.Count == 0)
            {
                throw new PinedaAppException("No Data");
            }

            List<TransactionResponse> responses = [];
            foreach(Transaction transaction in transactions)
            {
                TransactionResponse response = CreateTransactionResponse(transaction);
                responses.Add(response);
            }

            return CreateResponse("success", ("transaction", responses));
        }

        public Response UpsertTransaction(TransactionRequest request, out int newId, int? id = null)
        {
            Transaction transaction = BindTransactionFromRequest(request);
            Transaction toUpdate = null;
            double budgetValue;

            if (id != null)
            {
                toUpdate = _context.Transaction.FirstOrDefault(toUpdate => toUpdate.Id == id);
            }

            if (id == null && toUpdate == null) 
            {
               _context.Transaction.Add(transaction);
                budgetValue = transaction.Value;
            }
            else
            {
                budgetValue = transaction.Value - toUpdate.Value;
                toUpdate.Name = transaction.Name;
                toUpdate.Value = transaction.Value;
                toUpdate.BudgetId = transaction.BudgetId;
                toUpdate.CategoryId = transaction.CategoryId;
                toUpdate.LastUpdatedAt = DateTime.Now;

                _context.Transaction.Update(toUpdate);

                transaction = toUpdate;
            }

            if (transaction.BudgetId != null)
            {
                Budget budgetUpdate = _context.Budget.FirstOrDefault(b => b.Id == transaction.BudgetId) ?? throw new PinedaAppException($"Budget with id {transaction.BudgetId} not found");
                budgetUpdate.LastUpdatedAt = DateTime.Now;
                budgetUpdate.Current += budgetValue;
                _context.Budget.Update(budgetUpdate);
            }

            _context.SaveChanges();

            newId = transaction.Id;

            TransactionResponse response = CreateTransactionResponse(transaction);
            return CreateResponse("success", ("transaction", response));
        }

        private Transaction? BindTransactionFromRequest(TransactionRequest request)
        {
            ValidationErrors checks = ValidateTransaction(request);
            if (checks.HasErrors)
            {
                throw new PinedaAppException("Validation Error", 400, new ValidationException(checks));
            }

            Transaction transaction = new()
            {
                UserId = request.UserId,
                Name = request.Name,
                Value = request.Value,
                CategoryId = request.CategoryId,
                BudgetId = request.BudgetId,
                CreatedAt = DateTime.Now,
                LastUpdatedAt = DateTime.Now
            };

            _context.Entry(transaction).Reference(t => t.Category).Load();

            return transaction;
        }

        private ValidationErrors ValidateTransaction(TransactionRequest request)
        {
            ValidationErrors validationErrors = new();
            if (request == null)
            {
                validationErrors.AddError("The request is empty");
            }
            if (String.IsNullOrEmpty(request.Name))
            {
                validationErrors.AddError("Transaction Name is empty");
            }
            if(request.UserId == 0)
            {
                validationErrors.AddError("User Id is empty");
            }
            if(request.Value <= 0)
            {
                validationErrors.AddError("Cash nominal can't be zero or lower");
            }
            if(request.CategoryId == 0)
            {
                validationErrors.AddError("Category is empty");
            }
            if(request.CategoryId != 0)
            {
                bool tcExist = _context.TransactionCategory.Any(tc => tc.Id ==  request.CategoryId);
                if (!tcExist) validationErrors.AddError($"Transaction Cateogry with id {request.CategoryId} not found");
            }
            if(request.BudgetId != 0)
            {
                bool budgetExist = _context.Budget.Any(b => b.Id == request.BudgetId);
                if (!budgetExist) validationErrors.AddError($"Budget with id {request.BudgetId} not found");
            }

            return validationErrors;
        }

        private TransactionResponse CreateTransactionResponse(Transaction project)
        {
            TransactionResponse response = new 
            (
                project.Id,
                project.Name,
                project.Value,
                project.Category.Name,
                project.CreatedAt,
                project.LastUpdatedAt
            );

            return response;
        }
    }
}
