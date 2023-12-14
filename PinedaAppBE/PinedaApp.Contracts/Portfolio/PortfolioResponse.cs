namespace PinedaApp.Contracts
{
    public record PortfolioResponse
    (
        int Id,
        int UserId,
        string Name,
        string Description,
        string ImageFile,
        DateTime CreatedAt,
        DateTime LastUpdatedAt
    );
}
