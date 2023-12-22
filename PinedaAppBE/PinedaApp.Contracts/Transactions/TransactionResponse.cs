namespace PinedaApp.Contracts
{
    public record TransactionResponse
    (
        int Id,
        string Name,
        double Value,
        string Category,
        DateTime CreatedAt,
        DateTime UpdatedAt
    );
}
