using PinedaApp.Configurations;
using PinedaApp.Contracts;
using PinedaApp.Models;
using PinedaApp.Models.Errors;

namespace PinedaApp.Services
{
    public class ProjectHandledService(PinedaAppContext context) : BaseService, IProjectHandledService
    {
        private readonly PinedaAppContext _context = context;
        public void DeleteProjectHandled(int id)
        {
            ProjectHandled project = _context.ProjectHandled.FirstOrDefault(p => p.Id == id);
            if(project == null)
            {
                throw new PinedaAppException($"No Project data with id {id} to be deleted", 404);
            }

            _context.ProjectHandled.Remove(project);
            _context.SaveChanges();
        }

        public ProjectHandledResponse GetProjecthandled(int id)
        {
            ProjectHandled project = _context.ProjectHandled.FirstOrDefault(p => p.Id == id);
            if(project == null)
            {
                throw new PinedaAppException($"Project Handled with id {id} not found", 404);
            }

            return CreateProjectHandledResponse(project);
        }

        public List<ProjectHandledResponse> GetProjectHandled()
        {
            List<ProjectHandled> projectHandled = _context.ProjectHandled.ToList();
            if(projectHandled == null || projectHandled.Count == 0)
            {
                throw new PinedaAppException("No Data", 404);
            }

            List<ProjectHandledResponse> responses = new List<ProjectHandledResponse>();
            foreach (ProjectHandled project in projectHandled)
            {
                ProjectHandledResponse response = CreateProjectHandledResponse(project);
                responses.Add(response);
            }

            return responses;
        }

        public ProjectHandledResponse UpsertProjectHandled(ProjectHandledRequest request, int? id = null)
        {
            if (request == null) throw new PinedaAppException("No Request is Made", 400);
            ProjectHandled projectHandled = BindProjectHandledFromRequest(request);
            ProjectHandled toUpdate = null;

            if (id != null)
            {
                toUpdate = _context.ProjectHandled.FirstOrDefault(p => p.Id == id);
            }

            if (id == null || toUpdate == null)
            {
                _context.ProjectHandled.Add(projectHandled);
                _context.SaveChanges();
                return CreateProjectHandledResponse(projectHandled);
            }

            toUpdate.ProjectName = projectHandled.ProjectName;
            toUpdate.ProjectDescription = projectHandled.ProjectDescription;
            toUpdate.LastUpdatedAt = DateTime.Now;

            _context.ProjectHandled.Add(toUpdate);
            _context.SaveChanges();
            return CreateProjectHandledResponse(projectHandled);
        }

        private ProjectHandled? BindProjectHandledFromRequest(ProjectHandledRequest request)
        {
            ValidationErrors checks = ValidateProjectHandled(request);
            if (checks.HasErrors)
            {
                throw new PinedaAppException("Validation Error", 400, new ValidationException(checks));
            }

            ProjectHandled portfolio = new ProjectHandled()
            {
                ExperienceId = request.ExperienceId,
                ProjectName = request.ProjectName,
                ProjectDescription = request.ProjectDescription,
                CreatedAt = DateTime.Now,
                LastUpdatedAt = DateTime.Now
            };

            return portfolio;
        }

        private ValidationErrors ValidateProjectHandled(ProjectHandledRequest request)
        {
            ValidationErrors validationErrors = new();
            if (request == null)
            {
                validationErrors.AddError("The request is empty");
            }
            if (request.ExperienceId == 0)
            {
                validationErrors.AddError("Experience Id is Required");
            }
            if (request.ExperienceId > 0 && !_context.Experience.Any(e => e.Id == request.ExperienceId))
            {
                validationErrors.AddError($"Experience with Id: {request.ExperienceId} Not Found");
            }
            if (String.IsNullOrEmpty(request.ProjectName))
            {
                validationErrors.AddError("Project Name is empty");
            }

            return validationErrors;
        }

        private ProjectHandledResponse CreateProjectHandledResponse(ProjectHandled projectHandled)
        {
            ProjectHandledResponse response = new ProjectHandledResponse
            (
                projectHandled.Id,
                projectHandled.ExperienceId,
                projectHandled.ProjectName,
                projectHandled.ProjectDescription,
                projectHandled.CreatedAt,
                projectHandled.LastUpdatedAt
            );

            return response;
        }
    }
}
