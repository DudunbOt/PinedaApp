namespace PinedaApp.Contracts
{
    public record AcademicResponse
    (
        int Id,
        string SchoolName,
        string Degree,
        DateTime StartDate,
        DateTime EndDate,
        DateTime CreatedAt,
        DateTime LastUpdatedAt
    );
}
