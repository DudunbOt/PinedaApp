using AutoMapper;
using PinedaApp.Contracts;
using PinedaApp.Models.Errors;
using PinedaApp.Models;
using PinedaApp.Models.DTO;
using AutoMapper.QueryableExtensions;
using PinedaApp.Configurations;

namespace PinedaApp.Services
{
    public class ExperienceService(PinedaAppContext context, IMapper mapper) : ServiceBase(context, mapper), IExperienceServices
    {
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
            Experience experience = _context.Experience.FirstOrDefault(e => e.Id == id) ?? throw new PinedaAppException($"Experience with id: {id} not found", 404);
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

        public ExperienceResponse UpsertExperience(ExperienceRequest request, out int newId, int? id = null)
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
            }
            else
            {
                toUpdate.CompanyName = experience.CompanyName;
                toUpdate.Position = experience.Position;
                toUpdate.StartDate = experience.StartDate;
                toUpdate.ShortDesc = experience.ShortDesc;
                toUpdate.EndDate = experience.EndDate;
                toUpdate.LastUpdatedAt = DateTime.Now;

                _context.Experience.Update(toUpdate);
                experience = toUpdate;
            }

            _context.SaveChanges();

            newId = experience.Id;

            return CreateExperienceResponse(experience);
        }

        private Experience? BindExperienceFromRequest(ExperienceRequest request)
        {
            ValidationErrors checks = ValidateExperience(request);
            if (checks.HasErrors)
            {
                throw new PinedaAppException("Validation Error", 400, new ValidationException(checks));
            }

            DateTime startDate = ConvertDate(request.StartDate);
            DateTime endDate = ConvertDate(request.EndDate);

            Experience experience = new Experience()
            {
                UserId = request.UserId,
                CompanyName = request.CompanyName,
                Position = request.Position,
                ShortDesc = request.ShortDesc,
                StartDate = startDate,
                EndDate = endDate == DateTime.MinValue ? null : endDate,
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
            IEnumerable<ProjectDto> projects = _context.Project
                .Where(e => e.ExperienceId == experience.Id)
                .ProjectTo<ProjectDto>(_mapper.ConfigurationProvider);

            ExperienceResponse response = new ExperienceResponse
            (
                experience.Id,
                experience.CompanyName,
                experience.Position,
                experience.ShortDesc,
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