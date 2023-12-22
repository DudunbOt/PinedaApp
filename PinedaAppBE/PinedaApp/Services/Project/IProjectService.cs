using PinedaApp.Contracts;

namespace PinedaApp.Services
{
    public interface IProjectService : IServiceBase
    {
        Response GetProject();
        Response GetProject(int id);
        Response UpsertProject(ProjectRequest request, out int newId, int? id = null);
        void DeleteProject(int id);
    }
}
