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

        User newUser = BindUserFromRequest(request);
        User toUpdate = null;

        if (id != null)
        {
            toUpdate = _context.Users.FirstOrDefault(x => x.Id == id);
        }

        if (id == null || toUpdate == null)
        {
            _context.Users.Add(newUser);
            _context.SaveChanges();
            return CreateUserResponse(newUser);
        }


        toUpdate.UserName = newUser.UserName;
        toUpdate.Password = newUser.Password;
        toUpdate.FirstName = newUser.FirstName;
        toUpdate.LastName = newUser.LastName;
        toUpdate.Email = newUser.Email;
        toUpdate.Phone = newUser.Phone;
        toUpdate.Address = newUser.Address;
        toUpdate.LastUpdatedAt = DateTime.Now;

        _context.Update(toUpdate);
        _context.SaveChanges();
        return CreateUserResponse(toUpdate);
    }

    private User? BindUserFromRequest(UserRequest request)
    {
        ValidationErrors checks = ValidateUser(request);
        if (checks.HasErrors)
        {
            throw new PinedaAppException("Validation Error", 400, new ValidationException(checks));
        }

        User user = new()
        {
            UserName = request.UserName,
            Password = HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
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



        UserResponse response = new UserResponse
        (
            user.Id,
            user.UserName,
            user.FirstName,
            user.LastName,
            user.Email,
            user.Phone,
            user.Address,
            user.UserRole.ToString(),
            academics.Cast<object>().ToList(),
            experiences.Cast<object>().ToList(),
            portofolios.Cast<object>().ToList(),
            user.CreatedAt,
            user.LastUpdatedAt
        );

        return response;
    }

    private string GenerateToken(string secretKey, string issuer, string audience, IEnumerable<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.Now.AddHours(1), // Token expiration time
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}