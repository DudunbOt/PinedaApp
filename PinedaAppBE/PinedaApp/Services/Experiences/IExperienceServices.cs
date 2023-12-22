using PinedaApp.Contracts;

namespace PinedaApp.Services
{
    public interface IExperienceServices
    {
        Response GetExperiences();
        Response GetExperience(int id);
        Response UpsertExperience(ExperienceRequest request, out int newId, int? id = null);
        void DeleteExperience(int id);
    }
}