namespace PinedaApp.Contracts
{
    public record BudgetRequest
    (
        int UserId,
        string Name,
        double Goal,
        double Current
    );
}
