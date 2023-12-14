using PinedaApp.Contracts;

namespace PinedaApp.Services
{
    public interface IExperienceServices
    {
        List<ExperienceResponse> GetExperiences();
        ExperienceResponse GetExperience(int id);
        ExperienceResponse UpsertExperience(ExperienceRequest request, int? id = null);
        void DeleteExperience(int id);
    }
}