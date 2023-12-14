namespace PinedaApp.Contracts
{
    public record UserRequest
    (
        string UserName,
        string Password,
        string FirstName,
        string LastName,
        string Email,
        string Phone,
        string Address
    );
}
