using AutoMapper;
using PinedaApp.Configurations;
using PinedaApp.Contracts;
using PinedaApp.Models;
using PinedaApp.Models.Errors;
using System.Security.Cryptography.Xml;

namespace PinedaApp.Services
{
    public class TransactionCategoryService(PinedaAppContext context) : ServiceBase(context), ITransactionCategoryService
    {
        public void DeleteTransactionCategory(int id)
        {
            TransactionCategory transactionCategory = _context.TransactionCategory.FirstOrDefault(tc => tc.Id == id) ?? throw new PinedaAppException($"Transaction Category with id {id} not found");
            _context.TransactionCategory.Remove(transactionCategory);
            _context.SaveChanges();
        }

        public List<TransactionCategoryResponse> GetTransactionCategories()
        {
            List<TransactionCategory> transactionCategories = _context.TransactionCategory.ToList();
            if (transactionCategories == null || transactionCategories.Count == 0)
            {
                throw new PinedaAppException("No Data", 404);
            }

            List<TransactionCategoryResponse> responses = [];
            foreach(TransactionCategory transactionCategory in transactionCategories)
            {
                TransactionCategoryResponse response = CreateTransactionCategoryResponse(transactionCategory);
                responses.Add(response);
            }

            return responses;
            
        }

        public TransactionCategoryResponse GetTransactionCategory(int id)
        {
            TransactionCategory transactionCategory = _context.TransactionCategory.FirstOrDefault(tc => tc.Id ==id) ?? throw new PinedaAppException($"Transaction Category with id {id} not found");

            return CreateTransactionCategoryResponse(transactionCategory);
        }

        public TransactionCategoryResponse UpsertTransactionCategory(TransactionCategoryRequest request, out int newId, int? id = 0)
        {
            TransactionCategory transactionCategory = BindTransactionCategoryFromRequest(request);
            TransactionCategory toUpdate = new();

            if(id != null)
            {
                toUpdate = _context.TransactionCategory.FirstOrDefault(tc => tc.Id == id);
            }

            if(id == null || toUpdate == null)
            {
                _context.TransactionCategory.Add(transactionCategory);
            }
            else
            {
                toUpdate.Name = transactionCategory.Name;
                toUpdate.Description = transactionCategory.Description;
                toUpdate.Type = transactionCategory.Type;
                toUpdate.LastUpdatedAt = transactionCategory.LastUpdatedAt;

                transactionCategory = toUpdate;
                _context.TransactionCategory.Update(transactionCategory);
            }

            _context.SaveChanges();

            newId = transactionCategory.Id;

            return CreateTransactionCategoryResponse(transactionCategory);
        }

        private TransactionCategory? BindTransactionCategoryFromRequest(TransactionCategoryRequest request)
        {
            ValidationErrors checks = ValidateTransactionCategory(request);
            if (checks.HasErrors)
            {
                throw new PinedaAppException("Validation Error", 400, new ValidationException(checks));
            }

            TransactionCategory transactionCategory = new TransactionCategory()
            {
                Name = request.Name,
                Description = request.Description,
                Type = request.Type,
                CreatedAt = DateTime.Now,
                LastUpdatedAt = DateTime.Now
            };

            return transactionCategory;
        }

        private ValidationErrors ValidateTransactionCategory(TransactionCategoryRequest request)
        {
            ValidationErrors validationErrors = new();
            if (request == null)
            {
                validationErrors.AddError("The request is empty");
            }
           
            if (String.IsNullOrEmpty(request.Name))
            {
                validationErrors.AddError("Transaction Category Name is empty");
            }

            return validationErrors;
        }

        private TransactionCategoryResponse CreateTransactionCategoryResponse(TransactionCategory transactionCategory)
        {
            TransactionCategoryResponse response = new TransactionCategoryResponse
            (
                transactionCategory.Id,
                transactionCategory.Name,
                transactionCategory.Description,
                transactionCategory.Type,
                transactionCategory.CreatedAt,
                transactionCategory.LastUpdatedAt
            );

            return response;
        }
    }
}
