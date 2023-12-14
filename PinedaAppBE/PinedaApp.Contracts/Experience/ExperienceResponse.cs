namespace PinedaApp.Contracts
{
    public record ExperienceResponse
    (
        int Id,
        int UserId,
        string CompanyName,
        string Position,
        DateTime StartDate,
        DateTime? EndDate,
        DateTime CreatedAt,
        DateTime LastUpdatedAt,
        List<object> Projects
    );
}
