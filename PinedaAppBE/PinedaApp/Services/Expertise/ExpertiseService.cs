using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using PinedaApp.Configurations;
using PinedaApp.Contracts;
using PinedaApp.Models;
using PinedaApp.Models.DTO;
using PinedaApp.Models.Errors;

namespace PinedaApp.Services
{
    public class ExpertiseService(PinedaAppContext context) : BaseService, IExpertiseService
    {
        private readonly PinedaAppContext _context = context;

        public void DeleteExpertise(int id)
        {
            Expertise expertise = _context.Expertise.FirstOrDefault(x => x.Id == id);
            if (expertise == null)
            {
                throw new PinedaAppException($"Expertise with Id {id} not found");
            }

            _context.Expertise.Remove(expertise);
            _context.SaveChanges();
        }

        public Response GetExpertise(int id)
        {
            Expertise expertise = _context.Expertise.FirstOrDefault(x => x.Id == id);
            if (expertise == null)
            {
                throw new PinedaAppException($"Expertise with Id {id} not found");
            }

            ExpertiseResponse expertiseResponse = CreateExpertiseResponse(expertise);
            return CreateResponse("siccess", ("expertise", expertiseResponse));
        }

        public Response GetExpertises()
        {
            List<Expertise> expertises = _context.Expertise.ToList();
            if(expertises.Count == 0 || expertises == null)
            {
                throw new PinedaAppException("No Data", 404);
            }

            List<ExpertiseResponse> expertiseResponses = [];
            foreach(Expertise expertise in expertises)
            {
                ExpertiseResponse response = CreateExpertiseResponse(expertise);
                expertiseResponses.Add(response);
            }

            return CreateResponse("siccess", ("expertise", expertiseResponses));
        }

        public Response UpsertExpertise(ExpertiseRequest request, out int newId, int? id = null)
        {
            if (request == null) throw new PinedaAppException("No Request has been made", 400);
            Expertise expertise = BindExpertiseFromRequest(request);
            Expertise toUpdate = null;

            if(id != null)
            {
                toUpdate = _context.Expertise.FirstOrDefault(x => x.Id == id);
            }

            if (id == null || toUpdate == null)
            {
                bool isExist = _context.Expertise.Any(x => x.UserId == request.UserId);
                if(isExist)
                {
                    throw new PinedaAppException($"Expertise data with User Id {request.UserId} found. You can't make more than 1 Data. Update existing Data instead.", 400);
                }

                _context.Expertise.Add(expertise);
            } else
            {
                toUpdate.Skills = expertise.Skills;
                _context.Expertise.Update(toUpdate);
                expertise = toUpdate;
            }

            _context.SaveChanges();
            
            newId = expertise.Id;

            ExpertiseResponse expertiseResponse = CreateExpertiseResponse(expertise);
            return CreateResponse("siccess", ("expertise", expertiseResponse));
        }

        private Expertise? BindExpertiseFromRequest(ExpertiseRequest request)
        {
            ValidationErrors checks = ValidateExpertise(request);
            if (checks.HasErrors)
            {
                throw new PinedaAppException("Validation Error", 400, new ValidationException(checks));
            }

            Expertise Expertise = new Expertise()
            {
                UserId = request.UserId,
                Skills = request.Skills,
                CreatedAt = DateTime.Now,
                LastUpdatedAt = DateTime.Now
            };

            return Expertise;
        }

        private ValidationErrors ValidateExpertise(ExpertiseRequest request)
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
            if (String.IsNullOrEmpty(request.Skills))
            {
                validationErrors.AddError("Expertise Cannot be Empty is empty");
            }

            return validationErrors;
        }

        private ExpertiseResponse CreateExpertiseResponse(Expertise Expertise)
        {

            ExpertiseResponse response = new ExpertiseResponse
            (
                Expertise.Id,
                Expertise.UserId,
                Expertise.Skills,
                Expertise.CreatedAt,
                Expertise.LastUpdatedAt
            );

            return response;
        }
    }
}
