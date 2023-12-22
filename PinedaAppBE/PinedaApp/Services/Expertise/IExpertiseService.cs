using PinedaApp.Contracts;

namespace PinedaApp.Services
{
    public interface IExpertiseService : IServiceBase
    {
        Response GetExpertises();
        Response GetExpertise(int id);
        Response UpsertExpertise(ExpertiseRequest request, out int newId, int? id = null);
        void DeleteExpertise(int id);
    }
}
