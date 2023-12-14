namespace PinedaApp.Contracts
{
    public record AcademicRequest
    (
        int UserId,
        string SchoolName,
        string Degree,
        string StartDate,
        string EndDate
    );
}
