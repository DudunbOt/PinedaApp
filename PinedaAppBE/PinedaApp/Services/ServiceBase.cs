using AutoMapper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PinedaApp.Configurations;
using PinedaApp.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PinedaApp.Services
{
    public class ServiceBase
    {
        protected PinedaAppContext _context;
        protected IMapper? _mapper;
        protected AppSettings? _appSettings;

        public ServiceBase(PinedaAppContext context, IMapper mapper, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }
        public ServiceBase(PinedaAppContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public ServiceBase(PinedaAppContext context)
        {
            _context = context;
        }
        
        protected DateTime ConvertDate(string date)
        {
            if(string.IsNullOrEmpty(date)) return DateTime.MinValue;
            DateTime objDate;
            DateTime.TryParse(date, out objDate);

            return objDate;
        }
        protected string GenerateToken(string secretKey, string issuer, string audience, IEnumerable<Claim> claims)
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
        protected string UploadMediaFile(IFormFile file, string path1 = "", string path2 = "")
        {
            if (file == null) return null;

            string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", path1, path2);
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            string filename = null;
            using SHA256 sha256 = SHA256.Create();
            byte[] fileBytes = new byte[file.Length];
            using (Stream fs = file.OpenReadStream())
            {
                fs.Read(fileBytes, 0, (int)fileBytes.Length);
            }

            byte[] hashBytes = sha256.ComputeHash(fileBytes);
            string hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty);

            filename = hash + Path.GetExtension(file.FileName);
            string fullFilePath = Path.Combine(uploadFolder, filename);

            if (!File.Exists(fullFilePath))
            {
                using FileStream fileStream = new FileStream(fullFilePath, FileMode.Create);
                file.CopyTo(fileStream);
            }

            return filename;
        }

        protected static Response CreateResponse(string status, params (string key, object data)[] dataItems)
        {
            Dictionary<string, object> responseData = new Dictionary<string, object>();

            foreach (var dataItem in dataItems)
            {
                responseData.Add(dataItem.key, dataItem.data);
            }

            return new Response(status, responseData);
        }
    }
}
