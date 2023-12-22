using PinedaApp.Contracts;

namespace PinedaApp.Services;

public interface IUserService
{
    Response GetUsers();
    Response GetUser(int id);
    Response GetToken(string username, string password);
    Response UpsertUser(UserRequest user, out int newId, int? id = null);
    void DeleteUser(int id);
}