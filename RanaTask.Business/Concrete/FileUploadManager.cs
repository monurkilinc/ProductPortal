using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ProductPortal.Core.Utilities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductPortal.Business.Concrete
{
    public class FileUploadManager : IFileUploadService
    {
        private readonly IWebHostEnvironment _env;
        private const string UPLOAD_FOLDER = "uploads/products";

        public FileUploadManager(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, UPLOAD_FOLDER);
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return Path.Combine(UPLOAD_FOLDER, uniqueFileName);
        }

        public void DeleteFile(string filePath)
        {
            var fullPath = Path.Combine(_env.WebRootPath, filePath);
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }
}
