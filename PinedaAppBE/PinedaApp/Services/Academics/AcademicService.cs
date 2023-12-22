using PinedaApp.Configurations;
using PinedaApp.Contracts;
using PinedaApp.Models;
using PinedaApp.Models.Errors;

namespace PinedaApp.Services;

public class AcademicService: ServiceBase, IAcademicServices
{
    public AcademicService(PinedaAppContext context) : base(context) { }
    public void DeleteAcademic(int id)
    {
        Academic academic = _context.Academic.FirstOrDefault(a => a.Id == id);
        if (academic == null)
        {
            throw new PinedaAppException($"Academic with id: {id} Not Found", 400);
        }

        _context.Academic.Remove(academic);
        _context.SaveChanges();
    }

    public Response GetAcademic(int id)
    {
        Academic academic = _context.Academic.FirstOrDefault(a => a.Id == id);
        if (academic == null)
        {
            throw new PinedaAppException($"Academic with id: {id} Not Found", 400);
        }

        AcademicResponse academicResponse = CreateAcademicResponse(academic);
        return CreateResponse("success", ("academic", academicResponse));
    }

    public Response GetAcademics()
    {
        List<Academic> academics = _context.Academic.ToList();
        if (academics == null || academics.Count == 0)
        {
            throw new PinedaAppException("No Data", 404);
        }

        List<AcademicResponse> academicResponses = new List<AcademicResponse>();
        foreach (Academic academic in academics)
        {
            AcademicResponse response = CreateAcademicResponse(academic);
            academicResponses.Add(response);
        }

        return CreateResponse("success", ("academic", academicResponses));

    }

    public Response UpsertAcademic(AcademicRequest request, out int newId, int? id = null)
    {
        if (request == null) throw new PinedaAppException("No Request is made", 400);

        Academic academic = BindAcademicFromRequest(request);
        Academic toUpdate = null;

        if (id != null)
        {
            toUpdate = _context.Academic.FirstOrDefault(x => x.Id == id);
        }

        if (id == null || toUpdate == null)
        {
            _context.Academic.Add(academic);
        } 
        else
        {
            toUpdate.SchoolName = academic.SchoolName;
            toUpdate.Degree = academic.Degree;
            toUpdate.StartDate = academic.StartDate;
            toUpdate.EndDate = academic.EndDate;
            toUpdate.LastUpdatedAt = DateTime.Now;

            _context.Update(toUpdate);
            academic = toUpdate;
        }
       
        _context.SaveChanges();

        newId = academic.Id;

        AcademicResponse academicResponse = CreateAcademicResponse(academic);
        return CreateResponse("success", ("academic", academicResponse));
    }

    private Academic? BindAcademicFromRequest(AcademicRequest request)
    {
        ValidationErrors checks = ValidateAcademic(request);
        if (checks.HasErrors)
        {
            throw new PinedaAppException("Validation Error", 400, new ValidationException(checks));
        }

        DateTime startDate;
        DateTime endDate;

        if (!DateTime.TryParse(request.StartDate, out startDate))
        {
            throw new PinedaAppException("Start Date format must be (YYYY-MM-dd)", 400);
        }

        if (!DateTime.TryParse(request.EndDate, out endDate))
        {
            throw new PinedaAppException("End Date format must be (YYYY-MM-dd)", 400);
        }

        Academic academic = new Academic()
        {
            UserId = request.UserId,
            SchoolName = request.SchoolName,
            Degree = request.Degree,
            StartDate = startDate,
            EndDate = endDate,
            CreatedAt = DateTime.Now,
            LastUpdatedAt = DateTime.Now
        };

        return academic;
    }

    private ValidationErrors ValidateAcademic(AcademicRequest request)
    {
        ValidationErrors validationErrors = new ValidationErrors();
        if (request == null)
        {
            validationErrors.AddError("The request is empty");
        }
        if (request.UserId < 1)
        {
            validationErrors.AddError("User Id is Required");
        }
        if (!_context.Users.Any(u => u.Id == request.UserId))
        {
            validationErrors.AddError($"User with Id: {request.UserId} Not Found");
        }
        if (String.IsNullOrEmpty(request.SchoolName))
        {
            validationErrors.AddError("School Name is empty");
        }
        if (String.IsNullOrEmpty(request.Degree))
        {
            validationErrors.AddError("Degree is empty");
        }
        if (!DateTime.TryParse(request.StartDate, out _))
        {
            validationErrors.AddError("Start Date Format must be (YYYY-MM-dd)");
        }
        if (!String.IsNullOrEmpty(request.EndDate) && !DateTime.TryParse(request.StartDate, out _))
        {
            validationErrors.AddError("End Date Format must be (YYYY-MM-dd)");
        }

        return validationErrors;
    }

    private AcademicResponse CreateAcademicResponse(Academic Academic)
    {
        AcademicResponse response = new AcademicResponse
        (
            Academic.Id,
            Academic.UserId,
            Academic.SchoolName,
            Academic.Degree,
            Academic.StartDate,
            Academic.EndDate,
            Academic.CreatedAt,
            Academic.LastUpdatedAt
        );

        return response;
    }
}