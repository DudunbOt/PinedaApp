using Azure.Core;
using System.Security.Cryptography;

namespace PinedaApp.Services
{
    public class BaseService
    {
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

            return filename
        }
    }
}
