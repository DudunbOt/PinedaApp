using Microsoft.AspNetCore.SignalR;
using PinedaApp.Configurations;
using PinedaApp.Contracts;
using PinedaApp.Models;
using PinedaApp.Models.Errors;

namespace PinedaApp.Services
{
    public class TransactionService : ServiceBase, ITransactionService
    {
        public TransactionService(PinedaAppContext context) : base(context) { }

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

            if (id != null)
            {
                toUpdate = _context.Transaction.FirstOrDefault(toUpdate => toUpdate.Id == id);
            }

            if (id == null && toUpdate == null) 
            {
               _context.Transaction.Add(transaction); 
            }
            else
            {
                toUpdate.Name = transaction.Name;
                toUpdate.Value = transaction.Value;
                toUpdate.BudgetId = transaction.BudgetId;
                toUpdate.CategoryId = transaction.CategoryId;
                toUpdate.LastUpdatedAt = DateTime.Now;

                _context.Transaction.Update(toUpdate);

                transaction = toUpdate;
            }

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

            Transaction transaction = new Transaction()
            {
                UserId = request.UserId,
                Name = request.Name,
                Value = request.Value,
                CategoryId = request.CategoryId,
                BudgetId = request.BudgetId,
                CreatedAt = DateTime.Now,
                LastUpdatedAt = DateTime.Now
            };

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
