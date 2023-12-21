using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PinedaApp.Configurations;
using PinedaApp.Contracts;
using PinedaApp.Models;
using PinedaApp.Models.DTO;
using PinedaApp.Models.Errors;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PinedaApp.Services;

public class UserService : BaseService, IUserService
{
    private readonly PinedaAppContext _context;
    private readonly IMapper _mapper;
    private readonly AppSettings _appSettings;
    public UserService(PinedaAppContext context, IMapper mapper, IOptions<AppSettings> appSettings)
    {
        _context = context;
        _mapper = mapper;
        _appSettings = appSettings.Value;
    }

    public LoginResponse GetToken(string username, string password)
    {
        User user = _context.Users.FirstOrDefault(x => x.UserName == username && x.Password == HashPassword(password));
        if (user == null)
        {
            throw new PinedaAppException("Username / Password is Incorrect");
        }

        List<Claim> claims =
        [
            new(ClaimTypes.Name, username),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Role, user.UserRole.ToString())
        ];

        string token = GenerateToken(_appSettings.SecretKey, _appSettings.Issuer, _appSettings.Audience, claims);

        return new LoginResponse(token);
    }

    public List<UserResponse> GetUsers()
    {
        List<User> users = _context.Users.ToList();
        if (users == null || users.Count == 0)
        {
            throw new PinedaAppException("No Data", 404);
        }

        List<UserResponse> responses = new List<UserResponse>();
        foreach (User user in users)
        {
            UserResponse response = CreateUserResponse(user);
            responses.Add(response);
        }

        return responses;
    }

    public void DeleteUser(int id)
    {
        User toDelete = _context.Users.FirstOrDefault(x => x.Id == id);
        if (toDelete == null)
        {
            throw new PinedaAppException($"User with id {id} not found", 404);
        }

        _context.Users.Remove(toDelete);
        _context.SaveChanges();
    }

    public UserResponse GetUser(int id)
    {
        User user = _context.Users.FirstOrDefault(x => x.Id == id);
        if (user == null)
        {
            throw new PinedaAppException($"User with id {id} not found", 404);
        }
        return CreateUserResponse(user);
    }

    public UserResponse UpsertUser(UserRequest request, int? id = null)
    {
        if (request == null) throw new PinedaAppException("No Request is made", 400);

        User user = BindUserFromRequest(request);
        User toUpdate = null;

        if (id != null)
        {
            toUpdate = _context.Users.FirstOrDefault(x => x.Id == id);
        }

        if (id == null || toUpdate == null)
        {
            _context.Users.Add(user);
        }
        else
        {
            toUpdate.UserName = user.UserName;
            toUpdate.Password = user.Password;
            toUpdate.FirstName = user.FirstName;
            toUpdate.LastName = user.LastName;
            toUpdate.Bio = user.Bio;
            toUpdate.Email = user.Email;
            toUpdate.Phone = user.Phone;
            toUpdate.Address = user.Address;
            toUpdate.Occupation = user.Occupation;
            if (!string.IsNullOrEmpty(toUpdate.ProfilePicture) && user.ProfilePicture != null && toUpdate.ProfilePicture != user.ProfilePicture)
            {
                toUpdate.ProfilePicture = user.ProfilePicture;
            }
            toUpdate.LastUpdatedAt = DateTime.Now;

            _context.Update(toUpdate);
            user = toUpdate;

        }

        _context.SaveChanges();
        return CreateUserResponse(user);
    }

    private User? BindUserFromRequest(UserRequest request)
    {
        ValidationErrors checks = ValidateUser(request);
        if (checks.HasErrors)
        {
            throw new PinedaAppException("Validation Error", 400, new ValidationException(checks));
        }

        string pfpFileName = UploadMediaFile(request.ProfilePicture, "Users", "ProfilePicture");

        User user = new()
        {
            UserName = request.UserName,
            Password = HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Bio = request.Bio,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
            Occupation = request.Occupation,
            ProfilePicture = pfpFileName,
            UserRole = Role.User,
            CreatedAt = DateTime.Now,
            LastUpdatedAt = DateTime.Now,
        };

        return user;
    }

    private ValidationErrors ValidateUser(UserRequest request)
    {
        ValidationErrors validationErrors = new ValidationErrors();
        if (request == null)
        {
            validationErrors.AddError("The request is empty");
        }
        if (String.IsNullOrEmpty(request.UserName))
        {
            validationErrors.AddError("Username is empty");
        }
        if (String.IsNullOrEmpty(request.Password))
        {
            validationErrors.AddError("Password is empty");
        }
        if (request.Password.Length < 8)
        {
            validationErrors.AddError("Password must be longer than 8 characters");
        }

        return validationErrors;
    }

    private static string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = sha256.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2"));
            }

            return sb.ToString();
        }
    }

    private UserResponse CreateUserResponse(User user)
    {
        IEnumerable<AcademicDto> academics = _context.Academic
            .Where(a => a.UserId == user.Id)
            .ProjectTo<AcademicDto>(_mapper.ConfigurationProvider);

        IEnumerable<PortfolioDto> portofolios = _context.Portfolio
            .Where(a => a.UserId == user.Id)
            .ProjectTo<PortfolioDto>(_mapper.ConfigurationProvider);

        IEnumerable<ExperienceDto> experiences = _context.Experience
            .Where(a => a.UserId == user.Id)
            .ProjectTo<ExperienceDto>(_mapper.ConfigurationProvider);

        IEnumerable<ExpertiseDto> expertises = _context.Expertise
            .Where(a => a.UserId == user.Id)
            .ProjectTo<ExpertiseDto>(_mapper.ConfigurationProvider);

        ExpertiseDto expertise = expertises.FirstOrDefault();
        List<string> stringList = expertise.Skills.Split(',').ToList();

        UserResponse response = new UserResponse
        (
            user.Id,
            user.UserName,
            user.FirstName,
            user.LastName,
            user.Bio,
            user.Email,
            user.Phone,
            user.Address,
            user.UserRole.ToString(),
            user.Occupation,
            user.ProfilePicture,
            academics.Cast<object>().ToList(),
            experiences.Cast<object>().ToList(),
            portofolios.Cast<object>().ToList(),
            stringList,
            user.CreatedAt,
            user.LastUpdatedAt
        );

        return response;
    }
}