using PinedaApp.Contracts;

namespace PinedaApp.Services
{
    public interface IExpertiseService
    {
        List<ExpertiseResponse> GetExpertises();
        ExpertiseResponse GetExpertise(int id);
        ExpertiseResponse UpsertExpertise(ExpertiseRequest request, int? id = null);
        void DeleteExpertise(int id);
    }
}
