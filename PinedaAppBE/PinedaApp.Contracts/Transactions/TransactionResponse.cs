namespace PinedaApp.Contracts
{
    public record TransactionResponse
    (
        int Id,
        string Name,
        double Value,
        string Category,
        string Type
    );
}
