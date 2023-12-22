namespace PinedaApp.Contracts
{
    public record PortfolioResponse
    (
        int Id,
        string Name,
        string Description,
        string ImageFile,
        DateTime CreatedAt,
        DateTime LastUpdatedAt
    );
}
