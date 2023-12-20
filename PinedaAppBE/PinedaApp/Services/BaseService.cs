using Azure.Core;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PinedaApp.Services
{
    public class BaseService
    {
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

        public string UploadMediaFile(IFormFile file, string path1 = "", string path2 = "")
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
    }
}
