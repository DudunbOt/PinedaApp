namespace PinedaApp.Contracts
{
    public record AcademicResponse
    (
        int Id,
        int UserId,
        string SchoolName,
        string Degree,
        DateTime StartDate,
        DateTime EndDate,
        DateTime CreatedAt,
        DateTime LastUpdatedAt
    );
}
