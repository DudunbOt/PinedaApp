namespace PinedaApp.Contracts
{
    public record ExperienceRequest
    (
        int UserId,
        string CompanyName,
        string Position,
        string ShortDesc,
        string StartDate,
        string? EndDate
    );
}
