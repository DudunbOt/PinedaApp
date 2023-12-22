using PinedaApp.Configurations;
using PinedaApp.Contracts;
using PinedaApp.Models;
using PinedaApp.Models.Errors;

namespace PinedaApp.Services
{
    public class ProjectService(PinedaAppContext context) : BaseService, IProjectService
    {
        private readonly PinedaAppContext _context = context;
        public void DeleteProject(int id)
        {
            Project project = _context.Project.FirstOrDefault(p => p.Id == id);
            if(project == null)
            {
                throw new PinedaAppException($"No Project data with id {id} to be deleted", 404);
            }

            _context.Project.Remove(project);
            _context.SaveChanges();
        }

        public Response GetProject(int id)
        {
            Project project = _context.Project.FirstOrDefault(p => p.Id == id);
            if(project == null)
            {
                throw new PinedaAppException($"Project  with id {id} not found", 404);
            }

            ProjectResponse projectResponse = CreateProjectResponse(project);
            return CreateResponse("success", ("project", projectResponse));
        }

        public Response GetProject()
        {
            List<Project> projects = _context.Project.ToList();
            if(projects == null || projects.Count == 0)
            {
                throw new PinedaAppException("No Data", 404);
            }

            List<ProjectResponse> projectResponses = new List<ProjectResponse>();
            foreach (Project project in projects)
            {
                ProjectResponse response = CreateProjectResponse(project);
                projectResponses.Add(response);
            }

            return CreateResponse("success", ("project", projectResponses));
        }

        public Response UpsertProject(ProjectRequest request, out int newId, int? id = null)
        {
            if (request == null) throw new PinedaAppException("No Request is Made", 400);
            Project project = BindProjectFromRequest(request);
            Project toUpdate = null;

            if (id != null)
            {
                toUpdate = _context.Project.FirstOrDefault(p => p.Id == id);
            }

            if (id == null || toUpdate == null)
            {
                _context.Project.Add(project);
            }
            else
            {
                toUpdate.ProjectName = project.ProjectName;
                toUpdate.ProjectDescription = project.ProjectDescription;
                toUpdate.LastUpdatedAt = DateTime.Now;

                _context.Project.Update(toUpdate);
                project = toUpdate;
            }

            _context.SaveChanges();

            newId = project.Id;

            ProjectResponse projectResponse = CreateProjectResponse(project);
            return CreateResponse("success", ("project", projectResponse));
        }

        private Project? BindProjectFromRequest(ProjectRequest request)
        {
            ValidationErrors checks = ValidateProject(request);
            if (checks.HasErrors)
            {
                throw new PinedaAppException("Validation Error", 400, new ValidationException(checks));
            }

            Project portfolio = new Project()
            {
                ExperienceId = request.ExperienceId,
                ProjectName = request.ProjectName,
                ProjectDescription = request.ProjectDescription,
                CreatedAt = DateTime.Now,
                LastUpdatedAt = DateTime.Now
            };

            return portfolio;
        }

        private ValidationErrors ValidateProject(ProjectRequest request)
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

        private ProjectResponse CreateProjectResponse(Project project)
        {
            ProjectResponse response = new ProjectResponse
            (
                project.Id,
                project.ExperienceId,
                project.ProjectName,
                project.ProjectDescription,
                project.CreatedAt,
                project.LastUpdatedAt
            );

            return response;
        }
    }
}
