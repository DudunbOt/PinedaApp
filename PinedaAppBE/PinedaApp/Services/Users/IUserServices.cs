using PinedaApp.Contracts;

namespace PinedaApp.Services;

public interface IUserService
{
    List<UserResponse> GetUsers();
    UserResponse GetUser(int id);
    UserResponse UpsertUser(UserRequest user, int? id = null);
    void DeleteUser(int id);
}