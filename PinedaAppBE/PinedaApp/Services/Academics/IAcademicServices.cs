using PinedaApp.Contracts;

namespace PinedaApp.Services;

public interface IAcademicServices
{
    Response GetAcademics();
    Response GetAcademic(int id);
    Response UpsertAcademic(AcademicRequest request, out int newId, int? id = null);
    void DeleteAcademic(int id);
}