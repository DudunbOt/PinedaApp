namespace PinedaApp.Contracts
{
    public record UserResponse
    (
        int Id,
        string UserName,
        string FirstName,
        string LastName,
        string Email,
        string Phone,
        string Address,
        string Role,
        List<object> Academics,
        List<object> Experiences,
        List<object> Portofolios,
        DateTime CreatedAt,
        DateTime UpdatedAt
    );
}
