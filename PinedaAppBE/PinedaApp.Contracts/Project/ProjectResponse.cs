namespace PinedaApp.Contracts
{
    public record ProjectResponse
    (
        int Id,
        string ProjectName,
        string ProjectDescription,
        DateTime CreatedAt,
        DateTime LastUpdatedAt
    );
}
