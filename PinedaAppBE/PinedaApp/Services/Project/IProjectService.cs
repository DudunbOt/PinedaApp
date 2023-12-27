using PinedaApp.Contracts;

namespace PinedaApp.Services
{
    public interface IProjectService : IServiceBase
    {
        List<ProjectResponse> GetProjects();
        ProjectResponse GetProject(int id);
        ProjectResponse UpsertProject(ProjectRequest request, out int newId, int? id = null);
        void DeleteProject(int id);
    }
}
