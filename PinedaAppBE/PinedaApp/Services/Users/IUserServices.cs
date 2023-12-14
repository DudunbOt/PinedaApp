using PinedaApp.Contracts;

namespace PinedaApp.Services;

public interface IUserService
{
    List<UserResponse> GetUsers();
    UserResponse GetUser(int id);
    LoginResponse GetToken(string username, string password);
    UserResponse UpsertUser(UserRequest user, int? id = null);
    void DeleteUser(int id);
}