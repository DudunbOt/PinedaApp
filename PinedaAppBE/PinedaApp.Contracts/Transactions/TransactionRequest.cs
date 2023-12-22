namespace PinedaApp.Contracts
{
    public record TransactionRequest
    (
        string Name,
        double Value,
        int CategoryId
    );
}
