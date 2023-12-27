using PinedaApp.Contracts;

namespace PinedaApp.Services;

public interface IUserService : IServiceBase
{
    List<UserResponse> GetUsers();
    UserResponse GetUser(int id);
    LoginResponse GetToken(string username, string password);
    UserResponse UpsertUser(UserRequest user, out int newId, int? id = null);
    void DeleteUser(int id);
}