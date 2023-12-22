namespace PinedaApp.Contracts
{
    public record ExperienceResponse
    (
        int Id,
        string CompanyName,
        string Position,
        string ShortDesc,
        DateTime StartDate,
        DateTime? EndDate,
        DateTime CreatedAt,
        DateTime LastUpdatedAt,
        List<object> Projects
    );
}
