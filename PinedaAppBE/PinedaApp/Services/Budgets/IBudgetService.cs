﻿using PinedaApp.Contracts;

namespace PinedaApp.Services
{
    public interface IBudgetService :IServiceBase
    {
        Response GetBudgets();
        Response GetBudget(int id);
        Response UpsertBudget(BudgetRequest request, out int newId, int? id = null);
        void DeleteBudget(int id);
    }
}
