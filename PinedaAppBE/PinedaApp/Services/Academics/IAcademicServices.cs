using PinedaApp.Contracts;

namespace PinedaApp.Services;

public interface IAcademicServices
{
    List<AcademicResponse> GetAcademics();
    AcademicResponse GetAcademic(int id);
    AcademicResponse UpsertAcademic(AcademicRequest request, int? id = null);
    void DeleteAcademic(int id);
}