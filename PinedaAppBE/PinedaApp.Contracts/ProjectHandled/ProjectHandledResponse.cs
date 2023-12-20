namespace PinedaApp.Contracts
{
    public record ProjectHandledResponse
    (
        int Id,
        int ExperienceId,
        string ProjectName,
        string ProjectDescription,
        DateTime CreatedAt,
        DateTime LastUpdatedAt
    );
}
