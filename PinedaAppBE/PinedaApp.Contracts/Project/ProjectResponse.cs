namespace PinedaApp.Contracts
{
    public record ProjectResponse
    (
        int Id,
        int ExperienceId,
        string ProjectName,
        string ProjectDescription,
        DateTime CreatedAt,
        DateTime LastUpdatedAt
    );
}
