using PinedaApp.Contracts;

namespace PinedaApp.Services
{
    public interface IProjectService
    {
        Response GetProject();
        Response GetProject(int id);
        Response UpsertProject(ProjectRequest request, out int newId, int? id = null);
        void DeleteProject(int id);
    }
}
