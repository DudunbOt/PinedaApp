using PinedaApp.Contracts;

namespace PinedaApp.Services;

public interface IAcademicServices : IServiceBase
{
    List<AcademicResponse> GetAcademics();
    AcademicResponse GetAcademic(int id);
    AcademicResponse UpsertAcademic(AcademicRequest request, out int newId, int? id = null);
    void DeleteAcademic(int id);
}