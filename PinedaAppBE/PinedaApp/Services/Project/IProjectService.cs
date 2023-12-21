using PinedaApp.Contracts;

namespace PinedaApp.Services
{
    public interface IProjectService
    {
        List<ProjectResponse> GetProject();
        ProjectResponse GetProject(int id);
        ProjectResponse UpsertProject(ProjectRequest request, int? id = null);
        void DeleteProject(int id);
    }
}
