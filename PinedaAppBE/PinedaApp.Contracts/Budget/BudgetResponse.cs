namespace PinedaApp.Contracts
{
    public record BudgetResponse
    (
        int Id,
        string Name,
        double Goal,
        double Current,
        double Remaining,
        DateTime CreatedAt,
        DateTime LastUpdateAt,
        List<object> TransactionHistory
    );
}
