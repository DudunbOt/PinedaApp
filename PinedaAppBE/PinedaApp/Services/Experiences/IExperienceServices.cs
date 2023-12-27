using PinedaApp.Contracts;

namespace PinedaApp.Services
{
    public interface IExperienceServices : IServiceBase
    {
        List<ExperienceResponse> GetExperiences();
        ExperienceResponse GetExperience(int id);
        ExperienceResponse UpsertExperience(ExperienceRequest request, out int newId, int? id = null);
        void DeleteExperience(int id);
    }
}