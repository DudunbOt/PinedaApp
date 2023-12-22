namespace PinedaApp.Contracts
{
    public record TransactionCategoryResponse
    (
        int Id,
        string Name,
        string Description,
        string Type,
        DateTime CreatedAt,
        DateTime UpdatedAt
    );
}
