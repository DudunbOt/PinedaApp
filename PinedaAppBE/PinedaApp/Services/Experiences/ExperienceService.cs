using AutoMapper;
using PinedaApp.Contracts;
using PinedaApp.Models.Errors;
using PinedaApp.Models;
using PinedaApp.Models.DTO;
using AutoMapper.QueryableExtensions;
using PinedaApp.Configurations;

namespace PinedaApp.Services
{
    public class ExperienceService(PinedaAppContext context, IMapper mapper) : IExperienceServices
    {
        private readonly PinedaAppContext _context = context;
        private readonly IMapper _mapper = mapper;

        public void DeleteExperience(int id)
        {
            Experience experience = _context.Experience.FirstOrDefault(e => e.Id == id);
            if (experience == null)
            {
                throw new PinedaAppException($"Experience with id: {id} not found", 404);
            }

            _context.Experience.Remove(experience);
            _context.SaveChanges();
        }

        public ExperienceResponse GetExperience(int id)
        {
            Experience experience = _context.Experience.FirstOrDefault(e => e.Id == id);
            if (experience == null)
            {
                throw new PinedaAppException($"Experience with id: {id} not found", 404);
            }

            return CreateExperienceResponse(experience);
        }

        public List<ExperienceResponse> GetExperiences()
        {
            List<Experience> experiences = _context.Experience.ToList();
            if (experiences == null || experiences.Count == 0)
            {
                throw new PinedaAppException("No Data", 404);
            }

            List<ExperienceResponse> experienceResponses = new List<ExperienceResponse>();
            foreach (Experience experience in experiences)
            {
                ExperienceResponse response = CreateExperienceResponse(experience);
                experienceResponses.Add(response);
            }

            return experienceResponses;
        }

        public ExperienceResponse UpsertExperience(ExperienceRequest request, int? id = null)
        {
            if (request == null) throw new PinedaAppException("No request is made", 400);
            Experience experience = BindExperienceFromRequest(request);
            Experience toUpdate = null;

            if (id != null)
            {
                toUpdate = _context.Experience.FirstOrDefault(e => e.Id == id);
            }

            if (id == null || toUpdate == null)
            {
                _context.Experience.Add(experience);
                _context.SaveChanges();
                return CreateExperienceResponse(experience);
            }
            else
            {
                toUpdate.CompanyName = experience.CompanyName;
                toUpdate.Position = experience.Position;
                toUpdate.StartDate = experience.StartDate;
                toUpdate.EndDate = experience.EndDate;
                toUpdate.LastUpdatedAt = DateTime.Now;

                _context.Experience.Update(toUpdate);
                _context.SaveChanges();

                return CreateExperienceResponse(toUpdate);
            }
        }

        private Experience? BindExperienceFromRequest(ExperienceRequest request)
        {
            ValidationErrors checks = ValidateExperience(request);
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

            Experience experience = new Experience()
            {
                UserId = request.UserId,
                CompanyName = request.CompanyName,
                Position = request.Position,
                StartDate = startDate,
                EndDate = endDate,
                CreatedAt = DateTime.Now,
                LastUpdatedAt = DateTime.Now
            };

            return experience;
        }

        private ValidationErrors ValidateExperience(ExperienceRequest request)
        {
            ValidationErrors validationErrors = new ValidationErrors();
            if (request == null)
            {
                validationErrors.AddError("The request is empty");
            }
            if (request.UserId == 0)
            {
                validationErrors.AddError("User Id is Required");
            }
            if (request.UserId > 0 && !_context.Users.Any(u => u.Id == request.UserId))
            {
                validationErrors.AddError($"User with Id: {request.UserId} Not Found");
            }
            if (String.IsNullOrEmpty(request.CompanyName))
            {
                validationErrors.AddError("Company Name is empty");
            }
            if (String.IsNullOrEmpty(request.Position))
            {
                validationErrors.AddError("Position is empty");
            }
            if (!DateTime.TryParse(request.StartDate, out _))
            {
                validationErrors.AddError($"Start Date: {request.StartDate} Format must be (YYYY-MM-dd)");
            }
            if (!String.IsNullOrEmpty(request.EndDate) && !DateTime.TryParse(request.StartDate, out _))
            {
                validationErrors.AddError($"End Date: {request.EndDate} Format must be (YYYY-MM-dd)");
            }

            return validationErrors;
        }

        private ExperienceResponse CreateExperienceResponse(Experience experience)
        {
            IEnumerable<ProjectHandledDto> projects = _context.ProjectHandled
                .Where(e => e.ExperienceId == experience.Id)
                .ProjectTo<ProjectHandledDto>(_mapper.ConfigurationProvider);

            ExperienceResponse response = new ExperienceResponse
            (
                experience.Id,
                experience.UserId,
                experience.CompanyName,
                experience.Position,
                experience.StartDate,
                experience.EndDate,
                experience.CreatedAt,
                experience.LastUpdatedAt,
                projects.Cast<object>().ToList()
            );

            return response;
        }
    }
}