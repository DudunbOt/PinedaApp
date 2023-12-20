using PinedaApp.Contracts;

namespace PinedaApp.Services
{
    public interface IProjectHandledService
    {
        List<ProjectHandledResponse> GetProjectHandled();
        ProjectHandledResponse GetProjecthandled(int id);
        ProjectHandledResponse UpsertProjectHandled(ProjectHandledRequest request, int? id = null);
        void DeleteProjectHandled(int id);
    }
}
